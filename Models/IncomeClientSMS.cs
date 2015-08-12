using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSM.Models
{
    [Table("IncomeClientSMS")]
    public class IncomeClientSMS
    {
        public IncomeClientSMS()
        {
            DateTime = DateTime.Now;
        }

        public int IncomeClientSMSId { get; set; }

        

        [Required]
        public int MessageId { get; set; }

        [Required]
        public int ClientId { get; set; }

        public DateTime DateTime { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public string RecipientNumber { get; set; }

        [Required]
        public string SenderNumber { get; set; }

        [Required]
        public string SecretKey { get; set; }

        public virtual Client Client { get; set; }

        public virtual IncomeSMS IncomeSMS { get; set; }

     
        public IncomeClientSMSStatus Status { get; set; }
        public enum IncomeClientSMSStatus
        {
            Received,          
            Checked,
            Sent
        };

    }
}