namespace GSM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PlanRequest")]
    public partial class PlanRequest
    {
        public PlanRequest()
        {
            Date = DateTime.Now;
            Disabled = true;
        }

        public int PlanRequestId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public Client.Tariff RequestedPlan { get; set; }

        public bool Disabled { get; set; }

    public DateTime Date { get; set; }

        public virtual Client Client { get; set; }
    }
}
