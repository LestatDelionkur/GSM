using System;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.IO.Ports;
using System.Threading;
using GSM;
using System.Globalization;
using ImapX;
using ImapX.Enums;
using System.Net.Mail;
using GSM.Models;
using System.Collections.Generic;
using System.Net;
using System.Timers;
using HigLabo.Net.Mail;
using HigLabo.Net.Imap;
using HigLabo.Mime;
using NLog;
namespace GSM.Classess
{
    public class EmailBox
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public string Email { get; set; }
        public string Password { get; set; }
        public string ImapServer { get; set; }
        public string SmtpServer { get; set; }
        public int ImapPort { get; set; }
        public int SmtpPort { get; set; }
        public bool ImapSSL { get; set; }
        public bool SmtpSSL { get; set; }

        public ImapX.ImapClient ImapClient { get; set; }

        public System.Timers.Timer emailTimer { get; set; }
        public int processing = 0;

        public EmailBox(string Email, string Password, string ImapServer, string SmtpServer, int ImapPort, int SmtpPort, bool SmtpSSL, bool ImapSSL, int interval, ElapsedEventHandler handler)
        {
            this.Email = Email;
            this.Password = Password;
            this.ImapServer = ImapServer;
            this.SmtpServer = SmtpServer;
            this.ImapPort = ImapPort;
            this.SmtpPort = SmtpPort;
            this.ImapSSL = ImapSSL;
            this.SmtpSSL = SmtpSSL;
            emailTimer = new System.Timers.Timer(interval);
            this.emailTimer.Elapsed += handler;

        }
        public Message[] LoadMessages()
        {
            Message[] messages = null;
            try
            {
                bool connected = false;
                string username = Email;
                string password = Password;
                int port = ImapPort;
                bool useSSL = ImapSSL;
                try
                {
                    if (ImapClient == null) ImapClient = new ImapX.ImapClient(ImapServer, ImapPort, ImapSSL);

                    if (!ImapClient.IsConnected)
                        ImapClient.Connect();

                    if (ImapClient.IsConnected)
                    {
                        if (!ImapClient.IsAuthenticated)
                            ImapClient.Login(Email, Password);
                        if (ImapClient.IsAuthenticated)
                        {
                            connected = true;
                        }

                    }                   


                }
                catch (Exception e)
                {
                    logger.Error("Не удалось установить соединение и авторизоваться на сервере " + (ImapServer ?? "") + ".\r\nУдаленный порт: " + ImapPort + "\r\nШифрование:" + (ImapSSL ? "Включено" : "Выключено") + "\r\nТекст ошибки:", e);

                    connected = false;
                }

                if (connected)
                {
                    var folder = ImapClient.Folders.Inbox;
                    messages = folder.Search("UNSEEN", MessageFetchMode.Headers);
                    string deamon = "MAILER-DAEMON";
                    foreach (var mes in messages)
                    {
                        if (mes.From != null && mes.From.Address != null && mes.From.Address.ToLower().Contains(deamon.ToLower()))
                        {
                            mes.Seen = true;
                            mes.Remove();
                        }
                    }
                    messages = folder.Search("UNSEEN", MessageFetchMode.Basic);

                }
            }
            catch (Exception e)
            {
                logger.Error("Ошибка при  загрузке сообщений с почтового ящика " + (Email ?? "") + ".\r\nАдрес сервера:" + (ImapServer?? "") + ".\r\nУдаленный порт: " + ImapPort + "\r\nШифрование:" + (ImapSSL ? "Включено" : "Выключено") + "\r\nТекст ошибки:" ,e);
            }
            ImapClient = null;
            return messages;
        }

        public string GetTrubleMessageText(string sender, string subject, DateTime localDate)
        {
            string result = null;
            string server = ImapServer;


            try
            {
                using (HigLabo.Net.Imap.ImapClient cl = new HigLabo.Net.Imap.ImapClient(server))
                {
                    cl.UserName = Email;
                    cl.Password = Password;
                    cl.Port = ImapPort;
                    cl.Ssl = ImapSSL;
                    var bl = cl.Authenticate();

                    if (bl == true)
                    {
                        ImapFolder folder = cl.SelectFolder("INBOX");
                        SearchResult list = cl.ExecuteSearch("UNSEEN UNDELETED");
                        HigLabo.Mime.MailMessage mg = null;
                        for (int i = 0; i < list.MailIndexList.Count; i++)
                        {
                            mg = cl.GetMessage(list.MailIndexList[i]);
                            if (mg.Date.LocalDateTime == localDate && mg.From.Value.Equals(sender) && mg.Subject.Trim().Equals(subject))
                                break;
                        }

                        if (mg != null && mg.BodyText != null)
                            result = mg.BodyText;
                    }
                }

            }
            catch (Exception e)
            {
                logger.Error("Ошибка при повторной загрузке текста сообщения с почтового ящика " + (Email ?? "") + ".\r\nАдрес сервера:" + (server?? "") + ".\r\nУдаленный порт: " + ImapPort + "\r\nШифрование:" + (ImapSSL ? "Включено" : "Выключено") + "\r\nТекст ошибки:" , e);

            }
            return result;
        }

     

        public void SetSeen(List<Message> messages)
        {
            bool connected = false;
            string username = Email;
            string password = Password;
            int port = ImapPort;
            bool useSSL = ImapSSL;
            try
            {
                if (ImapClient == null) ImapClient = new ImapX.ImapClient(ImapServer, ImapPort, ImapSSL);

                if (!ImapClient.IsConnected)
                    ImapClient.Connect();

                if (ImapClient.IsConnected)
                {
                    if (!ImapClient.IsAuthenticated)
                        ImapClient.Login(Email, Password);
                    if (ImapClient.IsAuthenticated)
                    {
                        connected = true;
                    }

                }               


            }
            catch (Exception e)
            {
                logger.Error("Не удалось установить соединение и авторизоваться на сервере " + (ImapServer ?? "") + ".\r\nУдаленный порт: " + ImapPort + "\r\nШифрование:" + (ImapSSL ? "Включено" : "Выключено") + "\r\nТекст ошибки:", e);

                connected = false;
            }
            if (connected)
            {
                try
                {
                    var folder = ImapClient.Folders.Inbox;
                    foreach (var message in messages)
                    {

                        message.Seen = true;
                        message.Remove();

                    }
                }
                catch (Exception e)
                {
                    logger.Error("Ошибка при  отметке о чтении писем на почтовом ящике " + (Email ?? "") + ".\r\nАдрес сервера:" + (ImapServer?? "") + ".\r\nУдаленный порт: " + ImapPort + "\r\nШифрование:" + (ImapSSL ? "Включено" : "Выключено") + "\r\nТекст ошибки:" , e);
                }

            }
            ImapClient = null;
        }

     


        public void SendEmail(string Text, string Subject, string Address, GSM.MessageCenter.SendEmailDelegate callback, Object OriginalMessage)
        {
            try
            {
                SmtpClient SMTPClient = new SmtpClient(SmtpServer, SmtpPort);
                string server = SmtpServer;
                string username = Email;
                string password = Password;
                int port = SmtpPort;

                SMTPClient.EnableSsl = SmtpSSL;


                SMTPClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                SMTPClient.Host = server;
                SMTPClient.UseDefaultCredentials = false;
                SMTPClient.Credentials = new NetworkCredential(username, password);
                System.Net.Mail.MailAddress from = new System.Net.Mail.MailAddress(username, "Оцени-сервис", System.Text.Encoding.UTF8);
                System.Net.Mail.MailAddress to = new System.Net.Mail.MailAddress(Address);
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(from, to);

                message.Body = Text;
                message.IsBodyHtml = false;

                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Subject = Subject;
                message.SubjectEncoding = System.Text.Encoding.UTF8;


                SMTPClient.SendCompleted += new SendCompletedEventHandler(callback);
                SMTPClient.SendAsync(message, OriginalMessage);
            }
            catch (Exception e)
            {
                logger.Error("Ошибка при отправке сообщения с почтового ящика " + (Email ?? "") + ".\r\nАдрес сервера:" + (SmtpServer?? "") + ".\r\nУдаленный порт: " + SmtpPort + "\r\nШифрование:" + (ImapSSL ? "Включено" : "Выключено") + "\r\nТекст ошибки:" ,e);

            }

        }
    }
}



