namespace GSM.Models
{

    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Metadata.Edm;
    public partial class SMSContext : DbContext
    {
        public SMSContext()
            : base("name=SMSContext")
        {
        }

        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<ClientPhone> ClientPhone { get; set; }
        public virtual DbSet<IncomeSMS> IncomeSMS { get; set; }
        public virtual DbSet<PlanRequest> PlanRequest { get; set; }
        public virtual DbSet<ServicePhone> ServicePhone { get; set; }
        public virtual DbSet<Answer> Answer { get; set; }
        public virtual DbSet<IncomeClientWeb> IncomeClientWeb { get; set; }
        public virtual DbSet<IncomeClientEmail> IncomeClientEmail { get; set; }
        public virtual DbSet<IncomeClientSMS> IncomeClientSMS { get; set; }

        public virtual DbSet<Modem> Modem { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Client>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .Property(e => e.ShortKey)
                .IsUnicode(false);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.ClientPhone)
                .WithRequired(e => e.Client)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.PlanRequest)
                .WithRequired(e => e.Client)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.IncomeClientWeb)
                .WithRequired(e => e.Client)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.IncomeClientEmail)
                .WithRequired(e => e.Client)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.IncomeClientSMS)
                .WithRequired(e => e.Client)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<IncomeSMS>()
                .Property(e => e.SenderNumber)
                .IsUnicode(false);

            modelBuilder.Entity<IncomeSMS>()
                .Property(e => e.RecipientNumber)
                .IsUnicode(false);

            modelBuilder.Entity<IncomeSMS>()
                .Property(e => e.Text)
                .IsUnicode(false);

            modelBuilder.Entity<IncomeSMS>()
                .Property(e => e.ShortKey)
                .IsUnicode(false);

            modelBuilder.Entity<IncomeSMS>()
                .Property(e => e.SecretKey)
                .IsUnicode(false);

            modelBuilder.Entity<IncomeSMS>()
                .Property(e => e.Category)
                .IsUnicode(false);

            modelBuilder.Entity<ServicePhone>()
                .Property(e => e.PhoneNumber)
                .IsUnicode(false);

            modelBuilder.Entity<ServicePhone>()
                .HasMany(e => e.ClientPhone)
                .WithRequired(e => e.ServicePhone)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ServicePhone>()
               .HasMany(e => e.Modem)
               .WithRequired(e => e.ServicePhone)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<IncomeClientWeb>()
                .HasRequired(a => a.IncomeSMS)
                .WithMany()
                .HasForeignKey(a => a.MessageId);

            modelBuilder.Entity<Answer>().HasRequired(c => c.IncomeSMS).WithMany().HasForeignKey(c => c.MessageId);

            modelBuilder.Entity<IncomeSMS>().HasOptional(c => c.Answer).WithMany().HasForeignKey(c => c.AnswerId);

            modelBuilder.Entity<IncomeClientSMS>()
                .HasRequired(c => c.IncomeSMS)
                .WithMany()
                .HasForeignKey(c => c.MessageId);

            modelBuilder.Entity<IncomeClientEmail>()
                .HasRequired(c => c.IncomeSMS)
                .WithMany()
                .HasForeignKey(c => c.MessageId);

            modelBuilder.Entity<Client>()
                .HasMany(e => e.IncomeSMS)
                .WithRequired(c => c.Client)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Modem>()
            .HasKey(p => p.ModemId)
        .Property(p => p.ModemId).
        HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
           

        }
    }
}