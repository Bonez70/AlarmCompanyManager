using Microsoft.EntityFrameworkCore;
using AlarmCompanyManager.Models;

namespace AlarmCompanyManager.Data
{
    public class AlarmCompanyContext : DbContext
    {
        public AlarmCompanyContext(DbContextOptions<AlarmCompanyContext> options)
            : base(options)
        {
        }

        // Customer related DbSets
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactType> ContactTypes { get; set; }

        // Security System related DbSets
        public DbSet<SecuritySystem> SecuritySystems { get; set; }
        public DbSet<PanelType> PanelTypes { get; set; }
        public DbSet<MonitoringType> MonitoringTypes { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        public DbSet<CallListEntry> CallListEntries { get; set; }
        public DbSet<Communicator> Communicators { get; set; }
        public DbSet<CommunicatorType> CommunicatorTypes { get; set; }

        // Work Order related DbSets
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<WorkOrderType> WorkOrderTypes { get; set; }
        public DbSet<WorkOrderCategory> WorkOrderCategories { get; set; }
        public DbSet<WorkOrderStatus> WorkOrderStatuses { get; set; }
        public DbSet<Technician> Technicians { get; set; }
        public DbSet<WorkOrderItem> WorkOrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints
            ConfigureCustomerRelationships(modelBuilder);
            ConfigureSecuritySystemRelationships(modelBuilder);
            ConfigureWorkOrderRelationships(modelBuilder);
            ConfigureIndexes(modelBuilder);
            SeedData(modelBuilder);
        }

        private void ConfigureCustomerRelationships(ModelBuilder modelBuilder)
        {
            // Customer self-referencing relationship for linked customers
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.LinkedCustomer)
                .WithMany()
                .HasForeignKey(c => c.LinkedCustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer-Contact relationship
            modelBuilder.Entity<Contact>()
                .HasOne(c => c.Customer)
                .WithMany(c => c.Contacts)
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Customer-SecuritySystem relationship
            modelBuilder.Entity<SecuritySystem>()
                .HasOne(s => s.Customer)
                .WithMany(c => c.SecuritySystems)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureSecuritySystemRelationships(ModelBuilder modelBuilder)
        {
            // SecuritySystem-Zone relationship
            modelBuilder.Entity<Zone>()
                .HasOne(z => z.SecuritySystem)
                .WithMany(s => s.Zones)
                .HasForeignKey(z => z.SecuritySystemId)
                .OnDelete(DeleteBehavior.Cascade);

            // SecuritySystem-CallListEntry relationship
            modelBuilder.Entity<CallListEntry>()
                .HasOne(c => c.SecuritySystem)
                .WithMany(s => s.CallList)
                .HasForeignKey(c => c.SecuritySystemId)
                .OnDelete(DeleteBehavior.Cascade);

            // SecuritySystem-Communicator relationships
            modelBuilder.Entity<SecuritySystem>()
                .HasOne(s => s.PrimaryCommunicator)
                .WithMany(c => c.PrimarySecuritySystems)
                .HasForeignKey(s => s.PrimaryCommunicatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SecuritySystem>()
                .HasOne(s => s.SecondaryCommunicator)
                .WithMany(c => c.SecondarySecuritySystems)
                .HasForeignKey(s => s.SecondaryCommunicatorId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureWorkOrderRelationships(ModelBuilder modelBuilder)
        {
            // WorkOrder-Customer relationship
            modelBuilder.Entity<WorkOrder>()
                .HasOne(w => w.Customer)
                .WithMany(c => c.WorkOrders)
                .HasForeignKey(w => w.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkOrder-WorkOrderItem relationship
            modelBuilder.Entity<WorkOrderItem>()
                .HasOne(i => i.WorkOrder)
                .WithMany(w => w.WorkOrderItems)
                .HasForeignKey(i => i.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // WorkOrder-Technician relationship
            modelBuilder.Entity<WorkOrder>()
                .HasOne(w => w.Technician)
                .WithMany(t => t.WorkOrders)
                .HasForeignKey(w => w.TechnicianId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            // Customer indexes
            modelBuilder.Entity<Customer>()
                .HasIndex(c => new { c.LastName, c.FirstName })
                .HasDatabaseName("IX_Customer_Name");

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.EmailAddress)
                .HasDatabaseName("IX_Customer_Email");

            // Work Order indexes
            modelBuilder.Entity<WorkOrder>()
                .HasIndex(w => w.WorkOrderNumber)
                .IsUnique()
                .HasDatabaseName("IX_WorkOrder_Number");

            modelBuilder.Entity<WorkOrder>()
                .HasIndex(w => w.ScheduledDate)
                .HasDatabaseName("IX_WorkOrder_ScheduledDate");

            // Security System indexes
            modelBuilder.Entity<SecuritySystem>()
                .HasIndex(s => s.CentralStationNumber)
                .HasDatabaseName("IX_SecuritySystem_CentralStation");
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Customer Types
            modelBuilder.Entity<CustomerType>().HasData(
                new CustomerType { CustomerTypeId = 1, TypeName = "Residential", Description = "Residential customers" },
                new CustomerType { CustomerTypeId = 2, TypeName = "Commercial", Description = "Commercial customers" },
                new CustomerType { CustomerTypeId = 3, TypeName = "Government", Description = "Government entities" },
                new CustomerType { CustomerTypeId = 4, TypeName = "Education", Description = "Educational institutions" }
            );

            // Seed Contact Types
            modelBuilder.Entity<ContactType>().HasData(
                new ContactType { ContactTypeId = 1, TypeName = "Owner", Description = "Property owner" },
                new ContactType { ContactTypeId = 2, TypeName = "Primary", Description = "Primary contact" },
                new ContactType { ContactTypeId = 3, TypeName = "Secondary", Description = "Secondary contact" }
            );

            // Seed Monitoring Types
            modelBuilder.Entity<MonitoringType>().HasData(
                new MonitoringType { MonitoringTypeId = 1, TypeName = "Security Monitored", Description = "Security monitoring service" },
                new MonitoringType { MonitoringTypeId = 2, TypeName = "Fire UL Monitored", Description = "UL listed fire monitoring" },
                new MonitoringType { MonitoringTypeId = 3, TypeName = "Un Monitored", Description = "Local alarm only" }
            );

            // Seed Device Types
            modelBuilder.Entity<DeviceType>().HasData(
                new DeviceType { DeviceTypeId = 1, TypeName = "Door/Window", Description = "Door and window sensors" },
                new DeviceType { DeviceTypeId = 2, TypeName = "Motion", Description = "Motion detectors" },
                new DeviceType { DeviceTypeId = 3, TypeName = "Glass Break", Description = "Glass break detectors" },
                new DeviceType { DeviceTypeId = 4, TypeName = "Smoke", Description = "Smoke detectors" },
                new DeviceType { DeviceTypeId = 5, TypeName = "Heat", Description = "Heat detectors" },
                new DeviceType { DeviceTypeId = 6, TypeName = "Carbon Monoxide", Description = "CO detectors" },
                new DeviceType { DeviceTypeId = 7, TypeName = "Panic", Description = "Panic buttons" },
                new DeviceType { DeviceTypeId = 8, TypeName = "Medical", Description = "Medical alert devices" },
                new DeviceType { DeviceTypeId = 9, TypeName = "Other", Description = "Other device types" }
            );

            // Seed Communicator Types
            modelBuilder.Entity<CommunicatorType>().HasData(
                new CommunicatorType { CommunicatorTypeId = 1, TypeName = "GSM/Cell", Description = "Cellular communicator" },
                new CommunicatorType { CommunicatorTypeId = 2, TypeName = "AES", Description = "AES radio communicator" },
                new CommunicatorType { CommunicatorTypeId = 3, TypeName = "IP", Description = "Internet protocol communicator" },
                new CommunicatorType { CommunicatorTypeId = 4, TypeName = "POTS", Description = "Plain old telephone service" },
                new CommunicatorType { CommunicatorTypeId = 5, TypeName = "Other", Description = "Other communication methods" }
            );

            // Seed Work Order Types
            modelBuilder.Entity<WorkOrderType>().HasData(
                new WorkOrderType { WorkOrderTypeId = 1, TypeName = "Service Call", Description = "Service and repair calls" },
                new WorkOrderType { WorkOrderTypeId = 2, TypeName = "Installation", Description = "New system installations" },
                new WorkOrderType { WorkOrderTypeId = 3, TypeName = "Inspection", Description = "System inspections" }
            );

            // Seed Work Order Categories
            modelBuilder.Entity<WorkOrderCategory>().HasData(
                new WorkOrderCategory { CategoryId = 1, CategoryName = "Security System", Description = "Security alarm system work" },
                new WorkOrderCategory { CategoryId = 2, CategoryName = "Fire System", Description = "Fire alarm system work" },
                new WorkOrderCategory { CategoryId = 3, CategoryName = "CCTV", Description = "Video surveillance work" },
                new WorkOrderCategory { CategoryId = 4, CategoryName = "Access Control", Description = "Access control system work" }
            );

            // Seed Work Order Statuses
            modelBuilder.Entity<WorkOrderStatus>().HasData(
                new WorkOrderStatus { StatusId = 1, StatusName = "Unscheduled", Description = "Not yet scheduled", ColorCode = "#FF6B6B", SortOrder = 1 },
                new WorkOrderStatus { StatusId = 2, StatusName = "Scheduled", Description = "Scheduled for completion", ColorCode = "#4ECDC4", SortOrder = 2 },
                new WorkOrderStatus { StatusId = 3, StatusName = "In Progress", Description = "Currently being worked on", ColorCode = "#45B7D1", SortOrder = 3 },
                new WorkOrderStatus { StatusId = 4, StatusName = "Pending", Description = "Waiting for parts or approval", ColorCode = "#FFA726", SortOrder = 4 },
                new WorkOrderStatus { StatusId = 5, StatusName = "Canceled", Description = "Work order canceled", ColorCode = "#78909C", SortOrder = 5 },
                new WorkOrderStatus { StatusId = 6, StatusName = "Completed", Description = "Work completed successfully", ColorCode = "#66BB6A", SortOrder = 6 }
            );

            // Seed Panel Types
            modelBuilder.Entity<PanelType>().HasData(
                new PanelType { PanelTypeId = 1, Manufacturer = "Honeywell", ModelNumber = "VISTA-20P", Description = "VISTA-20P Control Panel" },
                new PanelType { PanelTypeId = 2, Manufacturer = "DSC", ModelNumber = "PC1864", Description = "PowerSeries PC1864 Panel" },
                new PanelType { PanelTypeId = 3, Manufacturer = "2GIG", ModelNumber = "GC3", Description = "2GIG Go!Control Panel" },
                new PanelType { PanelTypeId = 4, Manufacturer = "Qolsys", ModelNumber = "IQ Panel 2", Description = "Qolsys IQ Panel 2" }
            );
        }
    }
}