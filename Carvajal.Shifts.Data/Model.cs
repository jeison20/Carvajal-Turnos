namespace Carvajal.Shifts.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model : DbContext
    {
        public Model()
            : base("name=ModelData")
        {
        }

        public virtual DbSet<Advices> Advices { get; set; }
        public virtual DbSet<AdvicesProducts> AdvicesProducts { get; set; }
        public virtual DbSet<Centres> Centres { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<OrdersProducts> OrdersProducts { get; set; }
        public virtual DbSet<PruebaComercio> PruebaComercio { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Turns> Turns { get; set; }
        public virtual DbSet<TurnsProducts> TurnsProducts { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Exceptions> Exceptions { get; set; }
        public virtual DbSet<LinkedCentres> LinkedCentres { get; set; }
        public virtual DbSet<PruebaCE> PruebaCE { get; set; }
        public virtual DbSet<UnloadingTime> UnloadingTime { get; set; }
        public virtual DbSet<UsuariosTest> UsuariosTest { get; set; }
        public virtual DbSet<WorkingHours> WorkingHours { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Advices>()
                .Property(e => e.AdviceNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Advices>()
                .Property(e => e.Orders_OrderNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Advices>()
                .HasMany(e => e.AdvicesProducts)
                .WithOptional(e => e.Advices)
                .HasForeignKey(e => e.FkAdvices_Identifier);

            modelBuilder.Entity<AdvicesProducts>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<AdvicesProducts>()
                .Property(e => e.ReceivedAndAcceptedQuantity)
                .HasPrecision(14, 7);

            modelBuilder.Entity<Centres>()
                .Property(e => e.FirstDay)
                .IsFixedLength()
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

            modelBuilder.Entity<Centres>()
                .HasMany(e => e.LinkedCentres)
                .WithOptional(e => e.Centres)
                .HasForeignKey(e => e.FkCentres_Identifier);

            modelBuilder.Entity<Centres>()
                .HasMany(e => e.WorkingHours)
                .WithOptional(e => e.Centres)
                .HasForeignKey(e => e.FkCentres_Identifier);

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
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<OrdersProducts>()
                .Property(e => e.SplitQuantity)
                .HasPrecision(14, 7);

            modelBuilder.Entity<PruebaComercio>()
                .Property(e => e.NombreComercio)
                .IsUnicode(false);

            modelBuilder.Entity<Roles>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Status>()
                .Property(e => e.Name)
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
                .Property(e => e.InTurnQuantity)
                .HasPrecision(14, 7);

            modelBuilder.Entity<Users>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.Name)
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
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.FkUsers_Identifier);

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
                .HasMany(e => e.Exceptions)
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.FkUsers_Merchant_Identifier);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Exceptions1)
                .WithOptional(e => e.Users1)
                .HasForeignKey(e => e.FkUsers_Creator_Identifier);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.LinkedCentres)
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.FkUsers_Identifier);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.UnloadingTime)
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.FkUsers_Manufacturer_Identifier);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.UnloadingTime1)
                .WithOptional(e => e.Users1)
                .HasForeignKey(e => e.FkUsers_Merchant_Identifier);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.WorkingHours)
                .WithOptional(e => e.Users)
                .HasForeignKey(e => e.FkUsers_Merchant_Identifier);

            modelBuilder.Entity<PruebaCE>()
                .Property(e => e.NombreCE)
                .IsUnicode(false);

            modelBuilder.Entity<UnloadingTime>()
                .Property(e => e.AmountPerPallet)
                .HasPrecision(14, 7);

            modelBuilder.Entity<UnloadingTime>()
                .Property(e => e.PalletType)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<UsuariosTest>()
                .Property(e => e.Nombre)
                .IsUnicode(false);

            modelBuilder.Entity<UsuariosTest>()
                .Property(e => e.Clave)
                .IsUnicode(false);

            modelBuilder.Entity<WorkingHours>()
                .Property(e => e.WeekdayName)
                .IsFixedLength()
                .IsUnicode(false);
        }
    }
}
