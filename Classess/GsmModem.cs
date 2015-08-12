using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using GSM;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using GSM.Models;
using NLog;
using SMSPDULib;
namespace GSM
{
    public class GsmModem
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public SerialPort port 
        { 
            get 
            {
                return (_port ?? new SerialPort(PortName)); 
            }
            set
            {
                _port = value;
            }
            
        }
        private SerialPort _port;
        public ServicePhone ServicePhone { get; set; }
        private Mutex mutex;
        public string PortName { get; set; }
        public string ModemName { get; set; }
        public bool Busy {get;set;}
        public AutoResetEvent receiveNow { get; set; }
        public System.Timers.Timer modemTimer { get; set; }
        public int processing = 0 ;
        public GsmModem(int timerInterval, string PortName, ServicePhone ServicePhone, string ModemName)
        {                       
            this.Busy = false;
            this.port = new SerialPort(PortName);
            this.PortName = PortName;
            this.ModemName = ModemName;
            this.receiveNow = new AutoResetEvent(false);
            this.modemTimer = new System.Timers.Timer(timerInterval);
            this.port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            this.ServicePhone = ServicePhone;
            this.mutex = new Mutex();
        }

        public void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (e.EventType == SerialData.Chars)
                {
                    receiveNow.Set();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ModemIsEnable()
        {
            bool result = false;
            if (SerialPort.GetPortNames().Contains(PortName))
            {
                if (port.IsOpen && Busy)
                    result = true;
                else if (!port.IsOpen)
                {
                    try
                    {
                        port.Open();
                        string data = ExecCommand( "AT", 300, "Не удалось подключиться к модему");
                        if (data.Contains("OK"))
                            result = true;
                        port.Close();
                    }
                    catch
                    {
                        result = false;
                    }
                    finally
                    {
                        if (port.IsOpen)
                            port.Close();
                    }
                }
            }
            return result;
        }


        private string ExecCommand(string command, int responseTimeout, string errorMessage)
        {
            try
            {
                port.DiscardOutBuffer();
                port.DiscardInBuffer();
                receiveNow.Reset();
                port.Write(command + "\r");
                string input = ReadResponse(responseTimeout);
                if ((input.Length == 0) || ((!input.EndsWith("\r\n> ")) && (!input.EndsWith("\r\nOK\r\n"))))

                 logger.Warn("Не удалоcь загрузить сообщения с модема "+(this.ModemName??"")+", подключенного к порту "+(this.PortName?? "")+"\r\nОтвет модема:"+(input??""));
                return input;
            }
            catch (Exception ex)
            {
                logger.Error("Ошибка выполнения команды модемом " + (this.ModemName ?? "") + ", подключенного к порту " + (this.PortName ?? "") + ". " + errorMessage + "\r\nТекст ошибки:", ex);              
            }
            return null;
        }

        public string ReadResponse(int timeout)
        {
            string buffer = string.Empty;
            try
            {
                do
                {
                    if (receiveNow.WaitOne(timeout, false))
                    {
                        string t = port.ReadExisting();
                        buffer += t;
                    }                    
                }
                while (!buffer.EndsWith("\r\nOK\r\n") && !buffer.EndsWith("\r\n> ") && !buffer.EndsWith("\r\nERROR\r\n"));
            }
            catch (Exception ex)
            {
                throw ex;
            }         
            return buffer;
        }

        private string ConvertToUCS2(string txtInRus)
        {
            byte[] bytes = Encoding.GetEncoding("ucs-2").GetBytes(txtInRus);
            List<byte[]> byteList = new List<byte[]>();
            for (int i = 0; i < bytes.Length; i = i + 2)
            {
                byte temp_byte = bytes[i];
                bytes[i] = bytes[i + 1];
                bytes[i + 1] = temp_byte;
            }
            string res = BitConverter.ToString(bytes);
            res = res.Replace("-", "");
            return res;
        }

        private string ConvertRusFromUCS2(string txtInUCS)
        {
            StringBuilder RUS = new StringBuilder(txtInUCS.Length);
            for (int i = 0; i < txtInUCS.Length; i = i + 4)
            {
                string key = txtInUCS.Substring(i, 4);
                int decValue = int.Parse(key, System.Globalization.NumberStyles.HexNumber);
                RUS.Append(Char.ConvertFromUtf32(decValue));
            }
            return RUS.ToString();
        }

        public bool SendSMS(string senderPhone, string message)
        {
            mutex.WaitOne();
            if (message.Length > 140) message = message.Substring(0, 140);
            bool result = false;

            if (port.IsOpen)
            {

                port.Close();
            }


            try
            {
                port.Open();
            }
            catch (Exception e)
            {
                logger.Error("Ошибка соединения с модемом " + (this.ModemName ?? "") + ". Не удалось подключится к порту " + (this.PortName ?? "") + ". \r\nТекст ошибки:" ,e);        
            }
            if (port.IsOpen)
            {
                ExecCommand("AT", 300, "Не удалось подключиться к модему");            

               ExecCommand("AT+CMGF=0", 300, "Не удалось установить цифровой режим передачи.");
               ExecCommand("AT+CSCS=\"UCS2\"", 300, "Не удалось установить формат UCS2.");

                string pduData;
                SMS sms = new SMS();
                sms.Direction = SMSDirection.Submited;
                sms.PhoneNumber = senderPhone;
                sms.ValidityPeriod = new TimeSpan(4, 0, 0, 0);
                sms.Message = message;          
                pduData = sms.Compose(SMS.SMSEncoding.UCS2);
                ExecCommand("AT+CMGS=" + (pduData.Length / 2 - 1), 300, "Не удалось передать номер");

              string  recievedData = ExecCommand(pduData + "\x1A", 500, "Не удалось отправить сообщение"); //3 seconds

                if (recievedData.EndsWith("\r\nOK\r\n"))
                    result = true;
                else if (recievedData.Contains("ERROR"))
                {
                   logger.Warn("Не удалоcь отправить сообщение с модема " + (this.ModemName ?? "") + ", подключенного к порту " + (this.PortName?? "") + "\r\nОтвет модема:" + (recievedData ?? "")+"\r\nСовет: Проверте наличие денежнех средств на сим-карте");              
                    result = false;
                }
                if (port.IsOpen)
                {
                    port.Close();
                }
            }
            mutex.ReleaseMutex();
            return result;
        }

        public ShortMessageCollection ReadSMS()
        {
            mutex.WaitOne();
            ShortMessageCollection messages = null;
            if (port.IsOpen)
            {
                port.Close();
            }
            try
            {
                port.Open();
            }
            catch (Exception e)
            {
                string message = "Ошибка соединения с модемом " + (this.ModemName ?? String.Empty) + ". Не удалось подключится к порту " + (this.PortName ?? String.Empty) + ". \r\nТекст ошибки:";
                logger.Error(message, e);        
            }

            try
            {
                if (port.IsOpen)
                {
                    ExecCommand("AT", 300, "Не удалось подключится к модему");
                    ExecCommand( "AT+CMGF=1", 300, "Не удалось установить формат обмена");
                    ExecCommand( "AT+CSCS=\"UCS2\"", 300, "Не удалось установить формат UCS2.");
                    ExecCommand( "AT+CPMS=\"SM\"", 300, "Не удалось выбрать место хранения SIM");
                    string input = ExecCommand( "AT+CMGL=\"REC UNREAD\"", 1000, "Не считать новые сообщения из модема");
                    messages = ParseMessages(input);
                    DeleteSms();                    

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            mutex.ReleaseMutex();
            return messages;
            

        }

        public bool DeleteSms()
        {
            bool isDeleted = false;
            int uCountSMS = CountSMSmessages();
            if (uCountSMS > 0)
            {
                try
                {
                    ExecCommand("AT", 300, "Не удалось подключиться к модему");
                    ExecCommand( "AT+CMGF=1", 300, "Не удалось установить текстовый режим передачи.");
                    string recievedData = ExecCommand( "AT+CMGD=1,3", 300, "Ошибка при удалении");

                    if (recievedData.EndsWith("\r\nOK\r\n"))
                    {
                        isDeleted = true;

                    }
                    if (recievedData.Contains("ERROR"))
                    {
                        isDeleted = false;
                        logger.Warn("Не удалоcь удалить прочитанные сообщения с модема " + (this.ModemName ?? "") + ", подключенного к порту " + (this.PortName ?? "") + "\r\nОтвет модема:" + (recievedData ?? ""));                       
                    }
                    return isDeleted;
                }
                catch (Exception ex)
                {
                    logger.Error("Ошибка удаления  сообщений с модема " + (this.ModemName ?? "") + ", подключенного к порту " + (this.PortName?? "") + ". " +  "\r\nТекст ошибки:" , ex);
                }
            }
            return isDeleted;

        }

        public int CountSMSmessages()
        {
            int CountTotalMessages = 0;
            try
            {
                ExecCommand("AT", 300, "Не удалось подключиться к модему");
                ExecCommand( "AT+CMGF=1", 300, "Не удалось установить текстовый режим передачи.");
                string recievedData = ExecCommand( "AT+CPMS?", 500, "Не удалось получить количество  SMS ");
                int uReceivedDataLength = recievedData.Length;
                if (recievedData != null&&(recievedData.Length >= 45) )
                {
                    string[] strSplit = recievedData.Split(',');
                    string strMessageStorageArea1 = strSplit[0];
                    string strMessageExist1 = strSplit[1];
                    CountTotalMessages = Convert.ToInt32(strMessageExist1);

                }
                else
                {
                   logger.Warn("Не удалоcь получить количество сообщений с модема " + (this.ModemName ?? "") + ", подключенного к порту " + (this.PortName?? "") + "\r\nОтвет модема:" + (recievedData ?? ""));                

                }
                return CountTotalMessages;

            }
            catch (Exception ex)
            {
                logger.Error("Ошибка при получении количества сообщений с модема " + (this.ModemName ?? "") + ", подключенного к порту " + (this.PortName ?? "") + ". " + "\r\nТекст ошибки:",  ex);
                return 0;
            }

        }

        public ShortMessageCollection ParseMessages(string input)
        {
            ShortMessageCollection messages = new ShortMessageCollection();
            try
            {
                Regex r = new Regex(@"\+CMGL: (\d+),""(.+)"",""(.+)"",(.*),""(.+)""\r\n(.+)\r\n");
                Match m = r.Match(input);
                while (m.Success)
                {
                    ShortMessage msg = new ShortMessage();
                    msg.Index = m.Groups[1].Value;
                    msg.Status = m.Groups[2].Value;
                    msg.Sender = ConvertRusFromUCS2(m.Groups[3].Value);
                    msg.Alphabet = m.Groups[4].Value;
                    DateTime sourceDate = DateTime.ParseExact(m.Groups[5].Value, "dd/M/yy,HH:mm:ss+ff", CultureInfo.InvariantCulture);
                    msg.SentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, sourceDate.Hour, sourceDate.Minute, sourceDate.Second, sourceDate.Millisecond);
                    


                    msg.Text = ConvertRusFromUCS2(m.Groups[6].Value);

                    messages.Add(msg);
                    m = m.NextMatch();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка парсинга сообщений: " + ex.Message);
            }
            return messages;
        }
    }
}
