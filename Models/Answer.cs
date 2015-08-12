using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace GSM.Models
{
    [Table("Answer")]
    public class Answer
    {
        public int AnswerId { get; set; }

        [Required]
        public int MessageId { get; set; }

        [Required]
        public int AnswerMessageId { get; set; }

        public enum AnswerSource
        {
            Web, SMS, Email
        }

        [Required]
        public AnswerSource Source { get; set; }

        public IncomeSMS IncomeSMS { get; set; }
    }
}