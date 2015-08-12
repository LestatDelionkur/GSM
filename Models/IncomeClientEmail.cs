using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSM.Models
{
    [Table("IncomeClientEmail")]
    public class IncomeClientEmail
    {
        public IncomeClientEmail()
        {
            DateTime = DateTime.Now;
        }

        public int IncomeClientEmailId { get; set; }

        [Required]
        public int MessageId { get; set; }

        [Required]
        public int ClientId { get; set; }

        public DateTime DateTime { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public string RecipientEmail { get; set; }

        [Required]
        public string SenderEmail { get; set; }

        [Required]
        public string SecretKey { get; set; }

        public bool? IsSent { get; set; }
        public virtual Client Client { get; set; }

        public virtual IncomeSMS IncomeSMS { get; set; }
    }
}