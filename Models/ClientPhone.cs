namespace GSM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ClientPhone")]
    public partial class ClientPhone
    {
        public int ClientPhoneId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public int ServicePhoneId { get; set; }

        public virtual Client Client { get; set; }

        public virtual ServicePhone ServicePhone { get; set; }
    }
}
