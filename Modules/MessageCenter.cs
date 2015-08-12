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
using System.Timers;
using GSM.Models;
using System.Collections.Concurrent;
using System.Data.Entity;
using GSM.Classess;
using System.Net.Mail;
using ImapX;
using System.Data.Entity.Core.Objects;
using NLog;

namespace GSM
{
    public class MessageCenter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public ConcurrentDictionary<string, GsmModem> gsmModemList;
        public Dictionary<string, ShortMessageCollection> notProcessingSMS = new Dictionary<string, ShortMessageCollection>();
        public Mutex IncomeSMSSaveMutex = new Mutex();
        public EmailBox emailBox;
        public int modemInterval = 20000;
        public int emailInterval = 5000;
        public SMSContext globalSMSContext = new SMSContext();
        public Form parent;
        #region Init
        public bool Init(Form parent)
        {
            try
            {
                this.parent = parent;
                string Email = GSM.Settings.Default.Email;
                string Password = GSM.Settings.Default.Password;
                string ImapServer = GSM.Settings.Default.ImapServer;
                int ImapPort = GSM.Settings.Default.ImapPort;
                string SmtpServer = GSM.Settings.Default.SmtpServer;
                int SmtpPort = GSM.Settings.Default.SmtpPort;
                bool ImapSSL = GSM.Settings.Default.ImapSSL;
                bool SmtpSSL = GSM.Settings.Default.SmtpSSL;

                emailBox = new EmailBox(Email, Password, ImapServer, SmtpServer, ImapPort, SmtpPort, SmtpSSL, ImapSSL, emailInterval, new ElapsedEventHandler(EmailTimer_ElapsedEventHandler));

                List<Modem> modems = LoadModemList();
                gsmModemList = new ConcurrentDictionary<string, GsmModem>();
                foreach (Modem modemModel in modems)
                {
                    GsmModem modem = new GsmModem(modemInterval, modemModel.PortName, modemModel.ServicePhone, modemModel.ModemName);
                    modem.modemTimer.Elapsed += new ElapsedEventHandler((sender, e) => ModemTimer_ElapsedEventHandler(sender, e, modem));
                    modem.modemTimer.Start();
                    gsmModemList.AddOrUpdate(modem.ServicePhone.PhoneNumber, modem, (iindex, item) => modem);
                    notProcessingSMS.Add(modem.ServicePhone.PhoneNumber, new ShortMessageCollection());
                }


                emailBox.emailTimer.Start();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public List<Modem> LoadModemList()
        {
            List<Modem> modems;
            try
            {


                string Domain = Environment.MachineName;
                modems = globalSMSContext.Modem.Where(x => x.Deleted != true && x.DomainName == Domain).ToList();

                return modems;
            }
            catch (Exception e)
            {
                ((Form1)parent).Error(e);
                return new List<Modem>();
            }

        }

        #endregion


        #region TimerHandlers
        private void ModemTimer_ElapsedEventHandler(object sender, ElapsedEventArgs e, GsmModem modem)
        {
            ((System.Timers.Timer)sender).Stop();
            try
            {
                ModemHandleElapsed(sender, e, modem);
            }
            catch (Exception ex)
            {
                logger.Error("Ошибка выполнения операции с модемом " + (modem.ModemName ?? "") + ", подключенного к порту " + (modem.PortName ?? "") + ":", ex);
            }
            finally
            {
                modem.port.Dispose();
                ((System.Timers.Timer)sender).Enabled = true;
            }
        }

        private void ModemHandleElapsed(object sender, ElapsedEventArgs e, GsmModem modem)
        {
            int sync = Interlocked.CompareExchange(ref modem.processing, 1, 0);
            if (sync == 0)
            {
                SetChekedMessages(modem);
                ShortMessageCollection messages = modem.ReadSMS();
                if (messages != null && messages.Count > 0)
                    messages.Concat(notProcessingSMS[modem.ServicePhone.PhoneNumber]);
                else messages = notProcessingSMS[modem.ServicePhone.PhoneNumber];

                if (messages != null)
                {
                    if (messages.Count > 0)
                    {
                        messages = ProcessingNewIncomeSMSMessages(messages, modem);
                        foreach (ShortMessage message in messages)
                            message.IsRead = false;
                        notProcessingSMS[modem.ServicePhone.PhoneNumber] = messages;
                    }
                    ProcessingCheckedMessages(modem);
                    ProcessingNewIncomeClientWebMessages(modem);
                    ProcessingNotSentIncomeClientEmail(modem);
                    messages.Clear();
                    modem.processing = 0;
                }
            }
        }

        private void EmailTimer_ElapsedEventHandler(object sender, ElapsedEventArgs e)
        {
            ((System.Timers.Timer)sender).Stop();
            EmailHandleElapsed(sender, e);
            ((System.Timers.Timer)sender).Enabled = true;
        }

        private void EmailHandleElapsed(object sender, ElapsedEventArgs e)
        {
            int sync = Interlocked.CompareExchange(ref emailBox.processing, 1, 0);
            if (sync == 0)
            {
                List<Client> localClients = GetLocalClients();
                if (localClients != null && localClients.Count > 0)
                {
                    ImapX.Message[] emailMessages = emailBox.LoadMessages();
                    if (emailMessages != null && emailMessages.Count() > 0)
                        ProcesingNewIncomeClientEmail(emailMessages, localClients);

                }
                emailBox.processing = 0;
            }
        }

        # endregion




        #region ProcessingMessages
        public delegate void SendEmailDelegate(object sender, AsyncCompletedEventArgs e);

        public void SendEmailSetResult(object Token, AsyncCompletedEventArgs e)
        {
            IncomeSMSSaveMutex.WaitOne();
            using (SMSContext context = new SMSContext())
            {
                IncomeSMS incomeSMS = (IncomeSMS)e.UserState;
                incomeSMS.EmailIsSend = true;
                incomeSMS = ChangeIncomeSMS(incomeSMS, context);
                if (incomeSMS.SMSIsSend == true)
                    ChangeIncomeSMSStatus((IncomeSMS)e.UserState, IncomeSMS.IncomeSMSStatus.Sent, context);
            }
            IncomeSMSSaveMutex.ReleaseMutex();
        }

        public void SendIncomeClientSMSSetResult(object Token, AsyncCompletedEventArgs e)
        {
            using (SMSContext context = new SMSContext())
            {
                IncomeClientSMS incomeClientSMS = context.IncomeClientSMS.FirstOrDefault(x => x.IncomeClientSMSId == ((IncomeClientSMS)e.UserState).IncomeClientSMSId);
                ChangeIncomeClientSMSStatus(incomeClientSMS, IncomeClientSMS.IncomeClientSMSStatus.Sent, context);
            }
        }

        public void SendIncomeClientEmailAnswerSetResult(object Token, AsyncCompletedEventArgs e)
        {
            using (SMSContext context = new SMSContext())
            {
                IncomeClientEmail incomeClientEmail = context.IncomeClientEmail.FirstOrDefault(x => x.IncomeClientEmailId == ((IncomeClientEmail)e.UserState).IncomeClientEmailId);
                incomeClientEmail.IsSent = true;
                SaveIncomeClientEmail(incomeClientEmail, context);
            }
        }

        public void SendIncomeClientWebAnswerSetResult(object Token, AsyncCompletedEventArgs e)
        {
            using (SMSContext context = new SMSContext())
            {
                IncomeClientWeb incomeClientWeb = context.IncomeClientWeb.FirstOrDefault(x => x.IncomeClientWebId == ((IncomeClientWeb)e.UserState).IncomeClientWebId);
                incomeClientWeb.IsSent = true;
                SaveIncomeClientWebSendResult(incomeClientWeb, context);
            }
        }

        public ShortMessageCollection ProcessingNewIncomeSMSMessages(ShortMessageCollection messages, GsmModem modem)
        {
            IncomeSMSSaveMutex.WaitOne();
            ShortMessageCollection notSend = new ShortMessageCollection();
            try
            {
                using (SMSContext context = new SMSContext())
                {

                    ServicePhone ServicePhone = GetServicePhoneById(modem.ServicePhone.ServicePhoneId, context);
                    foreach (ShortMessage message in messages)
                    {
                        bool NotSaved = true;
                        bool isGarbage = true;
                        try
                        {
                            int firstWordLength = message.Text.Contains(" ") ? message.Text.Substring(0, message.Text.IndexOf(" ")).Length : 0;
                            string firstWord = message.Text.Contains(" ") ? message.Text.Substring(0, message.Text.IndexOf(" ")).Replace("\n", "").Replace("\r", "") : null;
                            Client client = GetClientByPhone(message.Sender, context);
                            bool haveFirstWord = !String.IsNullOrEmpty(firstWord);
                            bool ClientSMSAndNotAnswer = false;

                            if (client != null && haveFirstWord)
                            {
                                isGarbage = false;
                                IncomeSMS incomeSMS = GetIncomeSMS(firstWord, client.ClientId, context);
                                if (incomeSMS == null)
                                    ClientSMSAndNotAnswer = true;
                                else
                                {

                                    message.Recipient = incomeSMS.SenderNumber;
                                    message.Sender = ServicePhone.PhoneNumber;
                                    message.Text = message.Text.Substring(firstWordLength + 1);
                                    message.IsRead = true;
                                    if (ClientIsNotBlocked(client))
                                    {
                                        IncomeClientSMS incomeClientSMS = SaveIncomeClientSMS(message, ServicePhone, incomeSMS, client, context);
                                        if (incomeClientSMS != null) NotSaved = false;
                                        int answerId = AddAnswer(incomeSMS.MessageId, incomeClientSMS.IncomeClientSMSId, Answer.AnswerSource.SMS);
                                    }
                                }
                            }
                            if ((client == null || ClientSMSAndNotAnswer) && message.Sender != null && message.Sender.Length > 10)
                            {
                                isGarbage = false;
                                client = haveFirstWord ? GetClientByShortKey(firstWord, ServicePhone, context) : null;
                                string text;
                                if (ServicePhone.Type == ServicePhone.PhoneType.Private)
                                {
                                    client = GetClientByServicePhone(ServicePhone, context);
                                    text = message.Text;
                                }
                                else
                                {
                                    text = message.Text.Substring(firstWord.Length + 1);
                                }

                                IncomeSMS incomeSMS = null;

                                incomeSMS = context.IncomeSMS.FirstOrDefault(x => x.SenderNumber.Equals(message.Sender) && x.RecipientNumber.Equals(ServicePhone.PhoneNumber) && DbFunctions.DiffSeconds(x.DateTime, message.SentDate) <= 30 && x.Status != IncomeSMS.IncomeSMSStatus.Sent);

                                if (incomeSMS != null)
                                {
                                    incomeSMS.Text += message.Text;
                                    context.SaveChanges();
                                    NotSaved = false;
                                    message.IsRead = true;
                                }
                                else if (incomeSMS == null && client != null)
                                {
                                    if (ClientIsNotBlocked(client))
                                    {
                                        incomeSMS = SaveIncomeSMS(client, text, message.Sender, ServicePhone.PhoneNumber, message.SentDate, firstWord, context);
                                        if (incomeSMS != null) NotSaved = false;
                                    }
                                    message.IsRead = true;
                                }

                            }


                            if (NotSaved & !isGarbage)
                            {
                                notSend.Add(message);
                                message.IsRead = true;
                            }
                        }
                        catch
                        {
                            if (NotSaved & !isGarbage)
                            {
                                notSend.Add(message);
                                message.IsRead = true;
                            }
                        }

                    }

                    foreach (ShortMessage message in messages.Where(x => x.IsRead == false))
                    {
                        bool Saved = false;
                        try
                        {
                            Client client = GetClientByPhone(message.Sender, context);
                            if (client != null)
                            {
                                IncomeClientSMS incomeClientSMS = null;

                                incomeClientSMS = context.IncomeClientSMS.FirstOrDefault(x => x.Client.ClientId == client.ClientId && DbFunctions.DiffSeconds(x.DateTime, message.SentDate) <= 15 && x.Status != IncomeClientSMS.IncomeClientSMSStatus.Sent);



                                if (incomeClientSMS != null)
                                {

                                    incomeClientSMS.Text += message.Text;
                                    context.Entry(incomeClientSMS).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                    Saved = true;
                                }

                            }
                            else
                            {
                                IncomeSMS incomeSMS = null;

                                incomeSMS = context.IncomeSMS.FirstOrDefault(x => x.SenderNumber.Equals(message.Sender) && x.RecipientNumber.Equals(ServicePhone.PhoneNumber) && DbFunctions.DiffSeconds(x.DateTime, message.SentDate) <= 30 && x.Status != IncomeSMS.IncomeSMSStatus.Sent);

                                if (incomeSMS != null)
                                {
                                    incomeSMS.Text += message.Text;
                                    context.Entry(incomeSMS).State = System.Data.Entity.EntityState.Modified;
                                    context.SaveChanges();
                                    Saved = true;
                                }

                            }


                        }
                        catch
                        {
                            if (!Saved) notSend.Add(message);
                        }

                    }

                }
                return notSend;
            }
            catch (Exception e)
            {
                notSend = messages;
                return notSend;
            }
            finally
            {
                IncomeSMSSaveMutex.ReleaseMutex();
            }
        }

        public void ProcesingNewIncomeClientEmail(ImapX.Message[] emailMessages, List<Client> clients)
        {
            bool IsClientButNotAnswerFlag;

            using (SMSContext context = new SMSContext())
            {
                List<ImapX.Message> localUids = new List<ImapX.Message>();
                foreach (ImapX.Message message in emailMessages)
                {

                    IsClientButNotAnswerFlag = false;
                    if (message != null && message.From != null && !String.IsNullOrEmpty(message.From.Address))
                    {
                        Client client = clients.FirstOrDefault(x => x.Email.ToLower().Equals(message.From.Address.ToLower()));
                        if (client != null && ClientIsNotBlocked(client))
                        {
                            string SecretKey = message.Subject.Substring(message.Subject.IndexOf('№') + 1).Replace(" ", "").Replace("\u2192", "");
                            if (!String.IsNullOrEmpty(SecretKey) && message.Body != null)
                            {
                                IncomeSMS incomeSMS = GetIncomeSMS(SecretKey, client.ClientId, context);
                                if (incomeSMS != null)
                                {
                                    string text;

                                    string messageText = null;

                                    try
                                    {
                                        messageText = message.Body.Text;
                                    }
                                    catch
                                    {
                                        messageText = emailBox.GetTrubleMessageText(message.From.Address, message.Subject, (DateTime)message.Date);
                                    }

                                    if (messageText != null)
                                    {
                                        if (messageText.Contains("\r\n)\r\nIMAPX"))
                                            text = messageText.Substring(0, messageText.IndexOf("\r\n)\r\nIMAPX"));
                                        else
                                            text = messageText;
                                        if (text.Contains("\r\n"))
                                            text = text.Substring(0, text.IndexOf("\r\n"));

                                        IncomeClientEmail incomeClientEmail = new IncomeClientEmail()
                                        {

                                            ClientId = client.ClientId,
                                            DateTime = DateTime.Now,
                                            MessageId = incomeSMS.MessageId,
                                            IsSent = false,
                                            SenderEmail = client.Email,
                                            SecretKey = SecretKey,
                                            RecipientEmail = emailBox.Email,
                                            Text = text
                                        };
                                        incomeClientEmail = SaveIncomeClientEmail(incomeClientEmail, context);

                                        AddAnswer(incomeSMS.MessageId, incomeClientEmail.IncomeClientEmailId, Answer.AnswerSource.Email);

                                        if (incomeSMS.MessageType == IncomeSMS.Type.Email)
                                            emailBox.SendEmail(text, "Re:" + incomeSMS.ShortKey, incomeSMS.SenderAddress, SendIncomeClientEmailAnswerSetResult, incomeClientEmail);
                                        else
                                        {
                                            incomeClientEmail.IsSent = gsmModemList[incomeSMS.RecipientNumber].SendSMS(incomeSMS.SenderNumber, text);
                                            SaveIncomeClientEmail(incomeClientEmail, context);
                                        }

                                        localUids.Add(message);
                                    }
                                }
                                else
                                {
                                    IsClientButNotAnswerFlag = true;
                                }
                            }
                        }

                        if (client == null || IsClientButNotAnswerFlag)
                        {
                            string ShortKey = message.Subject.Replace(" ", "");
                            client = GetClientByShortKey(ShortKey, context);
                            if (client != null && ClientIsNotBlocked(client))
                            {
                                ServicePhone servicePhone = GetPhoneByClient(client, context);
                                if (servicePhone != null)
                                {
                                    string text;
                                    string messageText = null;

                                    try
                                    {
                                        messageText = message.Body.Text;
                                    }
                                    catch
                                    {
                                        messageText = emailBox.GetTrubleMessageText(message.From.Address, message.Subject, (DateTime)message.Date);
                                    }

                                    if (messageText != null)
                                    {

                                        if (messageText.Contains("\r\n)\r\nIMAPX"))
                                            text = messageText.Substring(0, messageText.IndexOf("\r\n)\r\nIMAPX"));
                                        else
                                            text = messageText;
                                        if (text.Contains("\r\n"))
                                            text = text.Substring(0, text.IndexOf("\r\n"));

                                        IncomeSMS incomeSMS = new IncomeSMS()
                                          {
                                              SenderAddress = message.From.Address.ToLower().Replace(" ", ""),
                                              RecipientNumber = servicePhone.PhoneNumber,
                                              Client = client,
                                              Text = text,
                                              DateTime = DateTime.Now,
                                              SecretKey = GetSecretKey(client),
                                              Status = IncomeSMS.IncomeSMSStatus.Checked,
                                              Category = "new",
                                              IsReaded = false,
                                              IsAnswered = false,
                                              SMSIsSend = false,
                                              ShortKey = ShortKey,
                                              MessageType = IncomeSMS.Type.Email
                                          };
                                        SaveIncomeSms(incomeSMS, context);
                                        localUids.Add(message);
                                    }

                                }
                            }
                        }

                    }
                }
                if (localUids.Count > 0) emailBox.SetSeen(localUids);
            }
        }
        public void ProcessingNotSentIncomeClientEmail(GsmModem modem)
        {
            using (SMSContext context = new SMSContext())
            {
                List<IncomeClientEmail> messages = GetNotSendIncomeClientEmail(modem, context);
                foreach (IncomeClientEmail message in messages)
                {
                    if (message.Client != null && ClientIsNotBlocked(message.Client))
                    {
                        IncomeSMS incomeSMS = GetIncomeSMS(message.SecretKey, message.Client.ClientId, context);
                        if (incomeSMS != null)
                        {
                            message.IsSent = modem.SendSMS(incomeSMS.SenderNumber, message.Text);
                            SaveIncomeClientEmail(message, context);
                        }
                    }
                }
            }
        }

        public void ProcessingNewIncomeClientWebMessages(GsmModem modem)
        {
            List<IncomeClientWeb> webMessages;

            using (SMSContext context = new SMSContext())
            {
                webMessages = context.IncomeClientWeb.Where(x => x.Client.ClientPhone.Select(y => y.ServicePhone.ServicePhoneId).Contains(modem.ServicePhone.ServicePhoneId) && x.IsSent != true && x.Client.IsBlocked != true).ToList();

            }

            foreach (IncomeClientWeb incomeClientWeb in webMessages)
            {
                IncomeSMS incomeSMS;

                using (SMSContext context = new SMSContext())
                {
                    incomeSMS = context.IncomeSMS.FirstOrDefault(x => x.MessageId == incomeClientWeb.MessageId);


                    if (incomeSMS != null)
                    {
                        if (incomeSMS.MessageType == IncomeSMS.Type.Email)
                        {
                            if (!String.IsNullOrEmpty(incomeSMS.SenderAddress))
                                emailBox.SendEmail(incomeClientWeb.Text, "Re:" + incomeSMS.ShortKey, incomeSMS.SenderAddress, SendIncomeClientWebAnswerSetResult, incomeClientWeb);

                        }
                        else
                            if (!String.IsNullOrEmpty(incomeSMS.SenderNumber))
                                if (modem.SendSMS(incomeSMS.SenderNumber, incomeClientWeb.Text))
                                    SaveIncomeClientWebSendResult(incomeClientWeb, context);
                    }
                }
            }

        }

        public void ProcessingCheckedMessages(GsmModem modem)
        {
            IncomeSMSSaveMutex.WaitOne();
            try
            {
                using (SMSContext context = new SMSContext())
                {
                    IList<IncomeSMS> IncomeSmsList = GetNotSendIncomeSMS(modem, context);

                    foreach (IncomeSMS incomeSMS in IncomeSmsList)
                    {
                        bool SMSresult = false;
                        if (incomeSMS.Client != null && incomeSMS.SMSIsSend != true)
                        {
                            SMSresult = modem.SendSMS(incomeSMS.Client.PhoneNumber, incomeSMS.SecretKey + " " + incomeSMS.Text);
                            incomeSMS.SMSIsSend = SMSresult;
                        }
                        else
                        {
                            incomeSMS.SMSIsSend = true;
                        }

                        if (incomeSMS.EmailIsSend != true) emailBox.SendEmail(incomeSMS.Text, "Сообщение №" + incomeSMS.SecretKey, incomeSMS.Client.Email, SendEmailSetResult, incomeSMS);

                        if (SMSresult && incomeSMS.EmailIsSend == true)

                            ChangeIncomeSMSStatus(incomeSMS, IncomeSMS.IncomeSMSStatus.Sent, context);
                        else
                            ChangeIncomeSMS(incomeSMS, context);
                    }

                    List<IncomeClientSMS> IncomeClientSmsList = GetIncomeSMSClient(modem, IncomeClientSMS.IncomeClientSMSStatus.Checked, context);

                    foreach (IncomeClientSMS IncomeClientSms in IncomeClientSmsList)
                    {
                        bool result;
                        if (IncomeClientSms.IncomeSMS.MessageType == IncomeSMS.Type.Email)
                            emailBox.SendEmail(IncomeClientSms.Text, "Re: " + IncomeClientSms.IncomeSMS.ShortKey, IncomeClientSms.IncomeSMS.SenderAddress, SendIncomeClientSMSSetResult, IncomeClientSms);
                        else
                        {
                            result = modem.SendSMS(IncomeClientSms.IncomeSMS.SenderNumber, IncomeClientSms.Text);
                            if (result)
                                ChangeIncomeClientSMSStatus(IncomeClientSms, IncomeClientSMS.IncomeClientSMSStatus.Sent, context);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                IncomeSMSSaveMutex.ReleaseMutex();
            }

        }
        # endregion


        #region  DatabaseOperations
        public void SetChekedMessages(GsmModem modem)
        {
            try
            {
                IList<IncomeSMS> incomeSMSs = null;
                IList<IncomeClientSMS> IncomeClientSMSs = null;
                using (SMSContext context = new SMSContext())
                {
                    incomeSMSs = context.IncomeSMS.Where(x => x.Status == IncomeSMS.IncomeSMSStatus.Received && x.RecipientNumber.Equals(modem.ServicePhone.PhoneNumber)).ToList();
                    IncomeClientSMSs = context.IncomeClientSMS.Where(x => x.Status == IncomeClientSMS.IncomeClientSMSStatus.Received && x.RecipientNumber.Equals(modem.ServicePhone.PhoneNumber)).ToList();
                }



                if (incomeSMSs != null)
                {
                    using (SMSContext context = new SMSContext())
                    {
                        foreach (IncomeSMS incomeSMS in incomeSMSs)
                        {
                            incomeSMS.Status = IncomeSMS.IncomeSMSStatus.Checked;
                            context.Entry(incomeSMS).State = System.Data.Entity.EntityState.Modified;
                        }
                        context.SaveChanges();
                    }

                }




                using (SMSContext context = new SMSContext())
                {
                    foreach (IncomeClientSMS incomeClientSMS in IncomeClientSMSs)
                    {

                        incomeClientSMS.Status = IncomeClientSMS.IncomeClientSMSStatus.Checked;
                        context.Entry(incomeClientSMS).State = System.Data.Entity.EntityState.Modified;

                    }
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.Error("Ошибка обработки базы данных. Не удалось обновить статусы сообщений.\r\nТекст ошибки:", e);

            }



        }
        private Client GetClientByPhone(string phoneNumber, SMSContext context)
        {
            Client queryResult;
            try
            {
                queryResult = context.Client.FirstOrDefault(x => x.PhoneNumber.Equals(phoneNumber));
            }
            catch (Exception e)
            {
                logger.Error("Ошибка чтения из базы данных. Не удалось получить клиента по номеру телефона.\r\nТекст ошибки:", e);
                queryResult = null;
            }
            return queryResult;
        }


        private ServicePhone GetPhoneByClient(Client client, SMSContext context)
        {
            ServicePhone queryResult;
            try
            {
                queryResult = context.ClientPhone.FirstOrDefault(x => x.ClientId == client.ClientId && !x.ServicePhone.IsDeleted).ServicePhone;

            }
            catch (Exception e)
            {
                logger.Error("Ошибка чтения из базы данных. Не удалось получить сервисный телефон по клиенту.\r\nТекст ошибки:", e);
                queryResult = null;
            }
            return queryResult;

        }

        private Client GetClientByShortKey(string ShortKey, ServicePhone Phone, SMSContext context)
        {
            try
            {
                Client queryResult;
                queryResult = context.Client.FirstOrDefault(x => x.ShortKey.ToLower().Trim().Equals(ShortKey.ToLower().Trim()) && x.ClientPhone.Select(y => y.ServicePhone.ServicePhoneId).Contains(Phone.ServicePhoneId));


                return queryResult;
            }
            catch (Exception e)
            {
                logger.Error("Ошибка чтения из базы данных. Не удалось получить клиента по ключу и сервисному телефону.\r\nТекст ошибки:", e);
                return null;
            }
        }

        private Client GetClientByShortKey(string ShortKey, SMSContext context)
        {
            try
            {
                Client queryResult;
                queryResult = context.Client.FirstOrDefault(x => x.ShortKey.ToLower().Replace(" ", "").Equals(ShortKey.ToLower()));
                return queryResult;
            }
            catch (Exception e)
            {
                logger.Error("Ошибка чтения из базы данных. Не удалось получить клиента по ключу.\r\nТекст ошибки:", e);
                return null;
            }
        }

        private Client GetClientByServicePhone(ServicePhone phone, SMSContext context)
        {

            try
            {
                Client queryResult;
                queryResult = context.ClientPhone.FirstOrDefault(x => x.ServicePhone.PhoneNumber.Equals(phone.PhoneNumber)).Client;


                return queryResult;
            }
            catch (Exception e)
            {
                logger.Error("Ошибка чтения из базы данных. Не удалось получить клиента по приватному  сервисному телефону.\r\nТекст ошибки:", e);
                return null;
            }
        }
        private Client GetClientByEmail(string email)
        {

            try
            {
                Client queryResult;
                using (SMSContext context = new SMSContext())
                {
                    queryResult = context.Client.FirstOrDefault(x => x.Email.Equals(email));

                }

                return queryResult;
            }
            catch (Exception e)
            {
                logger.Error("Ошибка чтения из базы данных. Не удалось получить клиента по E-mail.\r\nТекст ошибки:", e);
                return null;
            }
        }
        public List<IncomeSMS> GetNotSendIncomeSMS(GsmModem modem, SMSContext context)
        {

            try
            {
                List<IncomeSMS> list = context.IncomeSMS.Where(x => (x.Status == IncomeSMS.IncomeSMSStatus.Checked) && (x.RecipientNumber == modem.ServicePhone.PhoneNumber)).ToList();
                return list;
            }
            catch (Exception e)
            {
                logger.Error("Ошибка чтения из базы данных. Не удалось получить список не отправленных входящих сообщений.\r\nТекст ошибки:", e);
                return null;
            }

        }

        public List<IncomeClientSMS> GetIncomeSMSClient(GsmModem modem, IncomeClientSMS.IncomeClientSMSStatus Status, SMSContext context)
        {
            try
            {

                List<IncomeClientSMS> list = context.IncomeClientSMS.Where(x => (x.Status == Status) && x.RecipientNumber.Equals(modem.ServicePhone.PhoneNumber)).ToList();
                return list;
            }
            catch (Exception e)
            {
                logger.Error("Ошибка чтения из базы данных. Не удалось получить список не отправленных сообщений клиента.\r\nТекст ошибки:", e);
                return null;
            }


        }
        private IncomeClientSMS SaveIncomeClientSMS(ShortMessage message, ServicePhone ServicePhone, IncomeSMS incomeSMS, Client client, SMSContext context)
        {
            try
            {

                IncomeClientSMS incomeClientSMS = new IncomeClientSMS()
                {
                    Client = client,
                    ClientId = client.ClientId,
                    DateTime = message.SentDate,
                    Text = message.Text,
                    SenderNumber = client.PhoneNumber,
                    RecipientNumber = ServicePhone.PhoneNumber,
                    Status = IncomeClientSMS.IncomeClientSMSStatus.Received,
                    SecretKey = incomeSMS.SecretKey,
                    MessageId = incomeSMS.MessageId
                };

                context.IncomeClientSMS.Add(incomeClientSMS);
                context.SaveChanges();

                return incomeClientSMS;
            }
            catch (Exception e)
            {
                logger.Error("Ошибка записи  в базу данных. Не удалось добавить сообщение клиента с ответом.\r\nТекст ошибки:", e);
                return null;
            }
        }



        private IncomeClientEmail SaveIncomeClientEmail(IncomeClientEmail incomeClientEmail, SMSContext context)
        {
            try
            {
                if (incomeClientEmail.IncomeClientEmailId == 0)
                    context.IncomeClientEmail.Add(incomeClientEmail);
                else
                    context.Entry(incomeClientEmail).State = System.Data.Entity.EntityState.Modified;

                context.SaveChanges();

                return incomeClientEmail;
            }
            catch (Exception e)
            {
                logger.Error("Ошибка записи  в базу данных. Не удалось добавить  письмо клиента с ответом.\r\nТекст ошибки:", e);
                return null;
            }
        }

        private IncomeSMS SaveIncomeSMS(Client client, string text, string senderNumber, string recipienNumber, DateTime date, string ShortKey, SMSContext context)
        {
            try
            {
                IncomeSMS incomeSMS = new IncomeSMS()
                {
                    SenderNumber = senderNumber,
                    RecipientNumber = recipienNumber,
                    Client = client,
                    Text = text,
                    DateTime = date,
                    SecretKey = GetSecretKey(client),
                    Status = IncomeSMS.IncomeSMSStatus.Received,
                    Category = "new",
                    IsReaded = false,
                    IsAnswered = false,
                    SMSIsSend = false,
                    ShortKey = ShortKey,
                    MessageType = IncomeSMS.Type.SMS
                };
                context.IncomeSMS.Add(incomeSMS);
                context.SaveChanges();
                return incomeSMS;
            }
            catch (Exception e)
            {
                logger.Error("Ошибка записи  в базу данных. Не удалось добавить  входящее сообщения.\r\nТекст ошибки:", e);
                return null;
            }
        }

        private IncomeSMS SaveIncomeSms(IncomeSMS incomeSMS, SMSContext context)
        {

            try
            {
                if (incomeSMS.MessageId == 0)
                    context.IncomeSMS.Add(incomeSMS);
                else
                    context.Entry(incomeSMS).State = System.Data.Entity.EntityState.Modified;

                context.SaveChanges();

            }
            catch (Exception e)
            {
                logger.Error("Ошибка чтения  из базы данных.Не удалось сохранить  входящее сообщения.\r\nТекст ошибки:", e);
            }

            return incomeSMS;
        }



        private IncomeSMS ChangeIncomeSMSStatus(IncomeSMS incomeSMS, IncomeSMS.IncomeSMSStatus Status, SMSContext context)
        {
            try
            {
                incomeSMS.Status = Status;
                context.Entry(incomeSMS).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception e)
            {
                logger.Error("Ошибка записи  в базу данных. Не удалось сохранить изменение статуса входящего сообщения.\r\nТекст ошибки:", e);
            }

            return incomeSMS;
        }

        private IncomeSMS ChangeIncomeSMS(IncomeSMS incomeSMS, SMSContext context)
        {
            try
            {
                context.Entry(incomeSMS).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception e)
            {
                logger.Error("Ошибка записи  в базу данных. Не удалось сохранить изменения входящего сообщения.\r\nТекст ошибки:", e);
            }
            return incomeSMS;
        }

        private void SaveIncomeClientWebSendResult(IncomeClientWeb incomeClientWeb, SMSContext context)
        {
            try
            {
                incomeClientWeb.IsSent = true;
                context.Entry(incomeClientWeb).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception e)
            {
                logger.Error("Ошибка записи  в базу данных. Не удалось сохранить результат отправки сообщения с сайта.\r\nТекст ошибки:", e);
            }


        }

        private bool ClientIsNotBlocked(Client client)
        {
            try
            {
                using (SMSContext context = new SMSContext())
                {
                    List<ClientPhone> phones = context.ClientPhone.Where(x => x.ClientId == client.ClientId && x.ServicePhoneId == 1).ToList();
                    List<IncomeSMS> messages = context.IncomeSMS.Where(y => y.Client.ClientId == client.ClientId).ToList();

                    return client.IsBlocked != true && !(phones != null && messages.Count > 10);

                }
            }
            catch (Exception e)
            {
                logger.Error("Ошибка чтения из базы данных.Не удалось проверить доступ клиента.\r\nТекст ошибки:+" + geExceptionsMessage(e) ?? "" + "\r\n" + e.StackTrace);
                return true;
            }

        }
        private void ChangeIncomeClientSMSStatus(IncomeClientSMS incomeClientSMS, IncomeClientSMS.IncomeClientSMSStatus Status, SMSContext context)
        {
            try
            {
                incomeClientSMS.Status = Status;
                context.Entry(incomeClientSMS).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception e)
            {
                logger.Error("Ошибка записи в базу данных.Не удалось сохранить изменения статуса сообщения.\r\nТекст ошибки:", e);

            }


        }

        private void ChangeIncomeClientSMS(IncomeClientSMS incomeClientSMS, SMSContext context)
        {

            try
            {
                context.Entry(incomeClientSMS).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception e)
            {
                logger.Error("Ошибка записи в базу данных.Не удалось сохранить изменения сообщения.\r\nТекст ошибки:", e);

            }
        }

        private List<Client> GetLocalClients()
        {
            List<Client> clients = new List<Client>();

            try
            {
                List<int> ServicePhones = gsmModemList.Values.Select(y => y.ServicePhone.ServicePhoneId).ToList();

                foreach (var i in ServicePhones)
                {
                    using (SMSContext oContext = new SMSContext())
                    {
                        var clientPhone = oContext.ClientPhone.Where(c => c.ServicePhoneId == i).ToList();

                        var clientsAtPhone = clientPhone.Select(c => c.Client).ToList();

                        if (clientsAtPhone.Count() > 0)
                            clients.AddRange(clientsAtPhone);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Ошибка чтения  из базы данных.Не удалось загрузить клиентов привязанных к подключенным модемам." + "\r\nТекст ошибки:", e);
            }

            return clients;
        }

        private List<IncomeClientEmail> GetNotSendIncomeClientEmail(GsmModem modem, SMSContext context)
        {
            try
            {
                List<IncomeClientEmail> result = context.IncomeClientEmail.Where(x => x.Client.ClientPhone.Select(y => y.ServicePhone.ServicePhoneId).Contains(modem.ServicePhone.ServicePhoneId) && x.IsSent != true).ToList();
                return result;
            }
            catch (Exception e)
            {
                logger.Error("Ошибка чтения не отправленных сообщений из базы данных.\r\nТекст ошибки:", e);
                return null;
            }

        }


        private IncomeSMS GetIncomeSMS(string SecretKey, int ClientId, SMSContext context)
        {

            try
            {
                IncomeSMS queryResult = context.IncomeSMS.FirstOrDefault(x => x.SecretKey.Equals(SecretKey) && !((bool)(x.IsAnswered ?? false)) && x.Client.ClientId == ClientId);
                return queryResult;
            }
            catch (Exception e)
            {
                logger.Error("Ошибка чтения из базы данных.Не удалось загрузить входящее сообщение." + "\r\nТекст ошибки:", e);
                return null;
            }

        }

        private IncomeSMS GetIncomeSMS(int MessageId, SMSContext context)
        {
            try
            {
                IncomeSMS queryResult = context.IncomeSMS.FirstOrDefault(x => x.MessageId == MessageId);
                return queryResult;
            }
            catch (Exception e)
            {
                logger.Error("Ошибка чтения из базы данных.Не удалось загрузить входящее сообщение.", e);
                return null;
            }
        }

        private string GetSecretKey(Client client)
        {
            string secretKey = CreateSecretKey();
            try
            {
                using (SMSContext context = new SMSContext())
                {
                    while (context.IncomeSMS.Where(x => x.Client.ClientId == client.ClientId && x.SecretKey == secretKey).Count() > 0)
                    {
                        secretKey = CreateSecretKey();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Ошибка чтения из базы данных.Не удалось проверить секретный ключ на уникальность.", e);
            }

            return secretKey;
        }
        private string CreateSecretKey()
        {
            Random random = new Random();
            return random.Next(0, 10).ToString() + random.Next(0, 10).ToString() + random.Next(0, 10).ToString();
        }

        private int AddAnswer(int MessageId, int AnswerMessageId, Answer.AnswerSource Source)
        {
            Answer answer = new Answer();
            try
            {
                using (SMSContext context = new SMSContext())
                {
                    IncomeSMS queryResult = context.IncomeSMS.FirstOrDefault(x => x.MessageId == MessageId);
                    if (queryResult == null)
                        throw new Exception("Не найдено сообщения с номером" + MessageId);

                    queryResult.IsAnswered = true;
                    answer.IncomeSMS = queryResult;
                    answer.AnswerMessageId = AnswerMessageId;
                    answer.Source = Source;
                    context.Answer.Add(answer);

                    context.SaveChanges();

                    queryResult.AnswerId = answer.AnswerId;
                    context.Entry(queryResult).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logger.Error("Ошибка записи в базу данных.Не удалось сохранить ответ на сообщение." + "\r\nТекст ошибки:", e);
            }


            return answer.AnswerId;
        }

        private ServicePhone GetServicePhoneById(int ServicePhoneId, SMSContext context)
        {
            try
            {
                ServicePhone queryResult = context.ServicePhone.FirstOrDefault(x => x.ServicePhoneId == ServicePhoneId);
                return queryResult;
            }
            catch (Exception e)
            {
                logger.Error("Ошибка загрузки из базы данных.Не удалось получить сервисный телефон по идентификатору." + "\r\nТекст ошибки:", e);
                return null;
            }

        }

        private string geExceptionsMessage(Exception e)
        {
            Exception ex = e;
            string message = ex.Message;

            ex = ex.InnerException;
            while (ex != null)
            {
                message += "=>" + ex.Message;
                ex = ex.InnerException;
            }
            return message;

        }
        # endregion
    }
}
