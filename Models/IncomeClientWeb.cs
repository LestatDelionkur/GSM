using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace GSM.Models
{

    [Table("IncomeClientWeb")]
    public class IncomeClientWeb
    {
        public int IncomeClientWebId { get; set; }
       
        [Required]
        public int MessageId { get; set; }

        [Required]
        public int ClientId { get; set; }

        public DateTime DateTime { get; set; }

         [Required][MaxLength(140)]
        public string Text { get; set; }

         public bool? IsSent { get; set; }
        public virtual Client Client { get; set; }

        public virtual IncomeSMS IncomeSMS { get; set; }

        public IncomeClientWeb()
        {
            DateTime = DateTime.Now;
        }
    }
}