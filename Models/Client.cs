namespace GSM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Client")]

    public partial class Client
    {
        public Client()
        {
            ClientPhone = new HashSet<ClientPhone>();
            PlanRequest = new HashSet<PlanRequest>();
            IncomeClientWeb = new HashSet<IncomeClientWeb>();
            IncomeClientEmail = new HashSet<IncomeClientEmail>();
            IncomeClientSMS = new HashSet<IncomeClientSMS>();
            IncomeSMS = new HashSet<IncomeSMS>();
            RegistrationTime = DateTime.Now;
            TariffId = Tariff.Basic;

        }

        public int ClientId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Surname { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(15)]
        public string ShortKey { get; set; }

        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        public Tariff TariffId { get; set; }

        public DateTime RegistrationTime { get; set; }

        [NotMapped]
        public string UserName { get; set; }

        public bool IsBlocked { get; set; }

        public enum Tariff { Basic = 1, Standart = 2, Professional = 3 }

        public virtual ICollection<ClientPhone> ClientPhone { get; set; }

        public virtual ICollection<PlanRequest> PlanRequest { get; set; }

        public virtual ICollection<IncomeSMS> IncomeSMS { get; set; }
        public virtual ICollection<IncomeClientWeb> IncomeClientWeb { get; set; }
        public virtual ICollection<IncomeClientSMS> IncomeClientSMS { get; set; }
        public virtual ICollection<IncomeClientEmail> IncomeClientEmail { get; set; } 
    }

}
