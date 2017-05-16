namespace Carvajal.Shifts.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model : DbContext
    {
        public Model()
            : base("name=Model")
        {
        }

        public virtual DbSet<Advices> Advices { get; set; }
        public virtual DbSet<AdvicesProducts> AdvicesProducts { get; set; }
        public virtual DbSet<BlockingReasons> BlockingReasons { get; set; }
        public virtual DbSet<Centres> Centres { get; set; }
        public virtual DbSet<Countries> Countries { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<OrdersProducts> OrdersProducts { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Timezones> Timezones { get; set; }
        public virtual DbSet<Turns> Turns { get; set; }
        public virtual DbSet<TurnsProducts> TurnsProducts { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<Exceptions> Exceptions { get; set; }
        public virtual DbSet<TurnsAudit> TurnsAudit { get; set; }
        public virtual DbSet<UnloadingTime> UnloadingTime { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Advices>()
                .Property(e => e.FkUsers_Merchant_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<Advices>()
                .Property(e => e.FkUsers_Manufacturer_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<Advices>()
                .Property(e => e.AdviceNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Advices>()
                .Property(e => e.Orders_OrderNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Advices>()
                .Property(e => e.FkCentres_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<Advices>()
                .HasMany(e => e.AdvicesProducts)
                .WithOptional(e => e.Advices)
                .HasForeignKey(e => e.FkAdvices_Identifier);

            modelBuilder.Entity<AdvicesProducts>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<BlockingReasons>()
                .Property(e => e.PkIdentifier)
                .IsUnicode(false);

            modelBuilder.Entity<BlockingReasons>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<BlockingReasons>()
                .HasMany(e => e.Exceptions)
                .WithRequired(e => e.BlockingReasons)
                .HasForeignKey(e => e.FkBlockingReasons_Identifier)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Centres>()
                .Property(e => e.FkUsers_Merchant_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<Centres>()
                .Property(e => e.FkUsers_Responsable_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<Centres>()
                .Property(e => e.PkIdentifier)
                .IsUnicode(false);

            modelBuilder.Entity<Centres>()
                .Property(e => e.FirstDay)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Centres>()
                .Property(e => e.ListOfWorkingDays)
                .IsUnicode(false);

            modelBuilder.Entity<Centres>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Centres>()
                .HasMany(e => e.Advices)
                .WithOptional(e => e.Centres)
                .HasForeignKey(e => e.FkCentres_Identifier);

            modelBuilder.Entity<Centres>()
                .HasMany(e => e.Exceptions)
                .WithOptional(e => e.Centres)
                .HasForeignKey(e => e.FkCentres_Identifier);

            modelBuilder.Entity<Countries>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<Countries>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Countries>()
                .HasOptional(e => e.Address)
                .WithRequired(e => e.Countries);

            modelBuilder.Entity<Countries>()
                .HasMany(e => e.Timezones)
                .WithRequired(e => e.Countries)
                .HasForeignKey(e => e.FkCountries_Identifier)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Countries>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.Countries)
                .HasForeignKey(e => e.FkCountries_Identifier)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Orders>()
                .Property(e => e.FkUsers_Merchant_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<Orders>()
                .Property(e => e.FkUsers_Manufacturer_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<Orders>()
                .Property(e => e.OrderNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Orders>()
                .Property(e => e.OrderType)
                .IsUnicode(false);

            modelBuilder.Entity<Orders>()
                .HasMany(e => e.OrdersProducts)
                .WithOptional(e => e.Orders)
                .HasForeignKey(e => e.FkOrders_Identifier);

            modelBuilder.Entity<OrdersProducts>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<OrdersProducts>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Roles>()
                .Property(e => e.PkIdentifier)
                .IsUnicode(false);

            modelBuilder.Entity<Roles>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Roles>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.Roles)
                .HasForeignKey(e => e.FkRole_Identifier)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Status>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Status>()
                .HasMany(e => e.Orders)
                .WithOptional(e => e.Status)
                .HasForeignKey(e => e.FkStatus_Identifier);

            modelBuilder.Entity<Timezones>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Timezones>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<Timezones>()
                .HasMany(e => e.Centres)
                .WithRequired(e => e.Timezones)
                .HasForeignKey(e => e.FkTimezones_Identifier)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Timezones>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.Timezones)
                .HasForeignKey(e => e.FkTimezones_Identifier)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Turns>()
                .Property(e => e.FkUsers_Merchant_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<Turns>()
                .Property(e => e.FkUsers_Manufacturer_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<Turns>()
                .Property(e => e.FkUsers_Requester_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<Turns>()
                .Property(e => e.FkUsers_Modifier_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<Turns>()
                .Property(e => e.Orders_OrderNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Turns>()
                .Property(e => e.ReceivingAdvice_ReceivingAdviceNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Turns>()
                .HasMany(e => e.TurnsProducts)
                .WithOptional(e => e.Turns)
                .HasForeignKey(e => e.FkTurns_Identifier);

            modelBuilder.Entity<TurnsProducts>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.PkIdentifier)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.FkRole_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Advices)
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.FkUsers_Manufacturer_Identifier);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Advices1)
                .WithOptional(e => e.Users1)
                .HasForeignKey(e => e.FkUsers_Merchant_Identifier);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Centres)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.FkUsers_Merchant_Identifier)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Centres1)
                .WithOptional(e => e.Users1)
                .HasForeignKey(e => e.FkUsers_Responsable_Identifier);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Orders)
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.FkUsers_Manufacturer_Identifier);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Orders1)
                .WithOptional(e => e.Users1)
                .HasForeignKey(e => e.FkUsers_Merchant_Identifier);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Turns)
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.FkUsers_Manufacturer_Identifier);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Turns1)
                .WithOptional(e => e.Users1)
                .HasForeignKey(e => e.FkUsers_Merchant_Identifier);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Turns2)
                .WithOptional(e => e.Users2)
                .HasForeignKey(e => e.FkUsers_Modifier_Identifier);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Turns3)
                .WithOptional(e => e.Users3)
                .HasForeignKey(e => e.FkUsers_Requester_Identifier);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Address)
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.FkUsers_Identifier);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Exceptions)
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.FkUsers_Merchant_Identifier);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Exceptions1)
                .WithOptional(e => e.Users1)
                .HasForeignKey(e => e.FkUsers_Creator_Identifier);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.UnloadingTime)
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.FkUsers_Manufacturer_Identifier);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.UnloadingTime1)
                .WithOptional(e => e.Users1)
                .HasForeignKey(e => e.FkUsers_Merchant_Identifier);

            modelBuilder.Entity<Address>()
                .Property(e => e.FkUsers_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.AddressStreet)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.AddressNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.PostCode)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.Town)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.Region)
                .IsUnicode(false);

            modelBuilder.Entity<Address>()
                .Property(e => e.Country)
                .IsUnicode(false);

            modelBuilder.Entity<Exceptions>()
                .Property(e => e.FkUsers_Merchant_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<Exceptions>()
                .Property(e => e.FkCentres_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<Exceptions>()
                .Property(e => e.FkBlockingReasons_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<Exceptions>()
                .Property(e => e.FkUsers_Creator_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<TurnsAudit>()
                .Property(e => e.Subject)
                .IsUnicode(false);

            modelBuilder.Entity<TurnsAudit>()
                .Property(e => e.Message)
                .IsUnicode(false);

            modelBuilder.Entity<UnloadingTime>()
                .Property(e => e.FkUsers_Merchant_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<UnloadingTime>()
                .Property(e => e.FkUsers_Manufacturer_Identifier)
                .IsUnicode(false);

            modelBuilder.Entity<UnloadingTime>()
                .Property(e => e.ProductCode)
                .IsUnicode(false);

            modelBuilder.Entity<UnloadingTime>()
                .Property(e => e.AmountPerPallet)
                .HasPrecision(14, 7);

            modelBuilder.Entity<UnloadingTime>()
                .Property(e => e.PalletType)
                .IsFixedLength()
                .IsUnicode(false);
        }
    }
}
