using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HigLabo.Net.Mail;
using HigLabo.Net.Imap;
using HigLabo.Mime;
using System.Text.RegularExpressions;
using System.Net.Mail;
namespace GSM
{
    class Class1
    {
        public void init()
        {//Get unread message list from GMail
using (ImapClient cl = new ImapClient("imap.yandex.ru")) 
{
    cl.Port = 993;
    cl.Ssl = true; 
    cl.UserName = "noreply@oceni-service.ru";
    cl.Password = "3mpwBbmM3N";
    var bl = cl.Authenticate();
    if (bl == true)
    {
        //Select folder
        ImapFolder folder = cl.SelectFolder("INBOX");
        DateTime time;
        //Search Unread
        SearchResult list = cl.ExecuteSearch("UNSEEN UNDELETED");
       List< HigLabo.Mime.MailMessage > mg=new List<HigLabo.Mime.MailMessage>();
        //Get all unread mail
        for (int i = 0; i < list.MailIndexList.Count; i++)
        {
            mg.Add(cl.GetMessage(list.MailIndexList[i]));
        }
for (int i = 0; i < list.MailIndexList.Count; i++)
        {
            time = mg[i].Date.LocalDateTime;
        }
    }
    //Change mail read state as read
  //  cl.ExecuteStore(1, StoreItem.FlagsReplace, "UNSEEN");
}
        }
    }
}

 