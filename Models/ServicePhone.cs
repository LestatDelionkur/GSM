namespace GSM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ServicePhone")]
    public partial class ServicePhone
    {
        public ServicePhone()
        {
            ClientPhone = new HashSet<ClientPhone>();
        }

        public int ServicePhoneId { get; set; }

        [Required]
        [Display(Name = "Номер телефона")]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Тип телефона")]
        public PhoneType Type { get; set; }
	
        public bool IsDeleted { get; set; }
        public enum PhoneType
        {
            [Display(Name = "Публичный")]
            Public,
            [Display(Name = "Приватный")]
            Private
        };

        public virtual ICollection<ClientPhone> ClientPhone { get; set; }
        public virtual ICollection<Modem> Modem { get; set; }
    }
}
