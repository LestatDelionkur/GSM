using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
namespace GSM.Classess
{
   public  class EmailMessages
    {
        public IEnumerable<MailMessage> messages;
        public IEnumerable<uint> uids;
    }
}
