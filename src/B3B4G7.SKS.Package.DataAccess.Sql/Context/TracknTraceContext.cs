using B3B4G7.SKS.Package.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace B3B4G7.SKS.Package.DataAccess.Sql.Context
{
    public class TracknTraceContext : DbContext
    {

        public TracknTraceContext(DbContextOptions<TracknTraceContext> options)
        : base(options)
        {
        }

        public virtual DbSet<Parcel> Parcels { get; set; }
        public virtual DbSet<Hop> Hops { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }
        public virtual DbSet<Truck> Trucks { get; set; }
        public virtual DbSet<Transferwarehouse> Transferwarehouses { get; set; }
        public virtual DbSet<HopArrival> HopArrivals { get; set; }
        public virtual DbSet<WebhookResponse> WebhookResponses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /************** Relationships **************/
            modelBuilder.Entity<WarehouseNextHops>().HasOne(whnh => whnh.Hop).WithOne(h => h.WarehouseNextHops).HasForeignKey<WarehouseNextHops>(h => h.HopFK);
            modelBuilder.Entity<WarehouseNextHops>().HasOne(whnh => whnh.Parent).WithMany(h => h.NextHops).HasForeignKey(h => h.WarehouseFK).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Parcel>().HasOne(p => p.Recipient).WithMany().HasForeignKey(r => r.RecipientFK).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Parcel>().HasOne(p => p.Sender).WithMany().HasForeignKey(r => r.SenderFK).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Parcel>().HasMany(p => p.FutureHops).WithOne().HasForeignKey(ha => ha.FutureHopsFK).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Parcel>().HasMany(p => p.VisitedHops).WithOne().HasForeignKey(ha => ha.VisitedHopsFK).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transferwarehouse>().HasBaseType<Hop>();
            modelBuilder.Entity<Truck>().HasBaseType<Hop>();
            modelBuilder.Entity<Warehouse>().HasBaseType<Hop>();

            /****************** Hop ******************/
            var hop = modelBuilder.Entity<Hop>();
            hop.ToTable("Hops");
            hop.Property(h => h.HopType).IsRequired();
            hop.Property(h => h.Description).IsRequired();
            hop.Property(h => h.ProcessingDelayMins).IsRequired();
            hop.Property(h => h.LocationName).IsRequired();
            hop.Property(h => h.LocationCoordinates).IsRequired().HasColumnType("GEOMETRY");
            hop.HasKey(h => h.Code);


            /****************** HopArrival ******************/
            var hopArrival = modelBuilder.Entity<HopArrival>();
            hopArrival.ToTable("HopArrivals");
            hopArrival.Property(ha => ha.Description).IsRequired();
            //hopArrival.Property(ha => ha.DateTime).IsRequired();
            hopArrival.HasKey(ha => ha.HopArrivalId);


            /****************** Parcel ******************/
            var parcel = modelBuilder.Entity<Parcel>();
            parcel.ToTable("Parcels");
            parcel.Property(p => p.Weight).IsRequired();
            parcel.Property(p => p.State).IsRequired();
            parcel.HasKey(p => p.TrackingId);

            /****************** Recipient aka. Person ******************/
            var recipient = modelBuilder.Entity<Recipient>();
            recipient.ToTable("Persons");
            recipient.Property(r => r.Street).IsRequired();
            recipient.Property(r => r.PostalCode).IsRequired();
            recipient.Property(r => r.City).IsRequired();
            recipient.Property(r => r.Country).IsRequired();
            recipient.Property(r => r.Name).IsRequired();
            recipient.HasKey(r => r.PersonId);

            /****************** TransferWarehouse ******************/
            var transferWarehouse = modelBuilder.Entity<Transferwarehouse>();
            transferWarehouse.ToTable("TransferWarehouses");
            transferWarehouse.Property(tw => tw.Region).IsRequired().HasColumnType("GEOMETRY");
            transferWarehouse.Property(tw => tw.LogisticsPartner).IsRequired();
            transferWarehouse.Property(tw => tw.LogisticsPartnerUrl).IsRequired();

            /****************** Truck ******************/
            var truck = modelBuilder.Entity<Truck>();
            truck.ToTable("Trucks");
            truck.Property(t => t.Region).IsRequired().HasColumnType("GEOMETRY");
            truck.Property(t => t.NumberPlate).IsRequired();

            /****************** Warehouse ******************/
            var wareHouse = modelBuilder.Entity<Warehouse>();
            wareHouse.ToTable("Warehouses");
            wareHouse.Property(wh => wh.Level).IsRequired();

            /****************** WarehouseNextHops ******************/
            var wareHouseNextHops = modelBuilder.Entity<WarehouseNextHops>();
            wareHouseNextHops.ToTable("WarehouseNextHops");
            wareHouseNextHops.Property(whnh => whnh.TraveltimeMins).IsRequired();
            wareHouseNextHops.HasKey(whnh => whnh.WarehouseNextHopsId);

            /****************** WarehouseNextHops ******************/
            var webHooks = modelBuilder.Entity<WebhookResponse>();
            webHooks.ToTable("WebhookResponses");
            webHooks.HasKey(whnh => whnh.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
