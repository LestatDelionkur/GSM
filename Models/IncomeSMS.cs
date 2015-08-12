namespace GSM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IncomeSMS
    {
        [Key]
        public int MessageId { get; set; }

        
        [StringLength(15)]
        public string SenderNumber { get; set; }

        public string SenderAddress { get; set; }

        [Required]
        [StringLength(15)]
        public string RecipientNumber { get; set; }

        public DateTime DateTime { get; set; }

        [Required]
        public string Text { get; set; }


        [StringLength(15)]
        public string ShortKey { get; set; }

        [Required]
        [StringLength(15)]
        public string SecretKey { get; set; }

        public bool? IsAnswered { get; set; }

        public bool? IsReaded { get; set; }

        public IncomeSMSStatus Status { get; set; }

        [Required]
        [StringLength(255)]
        public string Category { get; set; }

        [NotMapped]
        public string AnswerText { get; set; }

        public int? AnswerId { get; set; }

        public virtual Client Client { get; set; }
        public virtual IncomeClientWeb IncomeClientWeb { get; set; }
        public virtual Answer Answer { get; set; }

        public bool? EmailIsSend { get; set; }
        public bool? SMSIsSend { get; set; }

        public Type MessageType { get; set; }
        public enum IncomeSMSStatus
        {
            Received,
            Checked,
            Sent
        };

        public enum Type
        {
            SMS,
            Email
        }
    }
}
