using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSM.Models
{
    
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table ("Modem")]
    public class Modem
    {
       
       
        public int ModemId { get; set; }

        public string PortName{get;set;}       
        public string ModemName { get; set; }
        public string DomainName { get; set; }
        public bool Deleted { get; set; }
        public int ServicePhoneId { get; set; }


        public virtual ServicePhone ServicePhone { get; set; }
    }
}
