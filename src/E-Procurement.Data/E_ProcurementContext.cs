using E_Procurement.Data.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace E_Procurement.Data
{
    public class EProcurementContext: IdentityDbContext<User,Role,int>
    {
        public EProcurementContext(DbContextOptions<EProcurementContext> options):base(options)
        {
            
        }

        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<VendorCategory> VendorCategories { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<DNGeneration> DnGenerations { get; set; }
        public DbSet<GRNGeneration> GrnGenerations { get; set; }
        public DbSet<POGeneration> PoGenerations { get; set; }
        public DbSet<RFQApprovalConfig> RfqApprovalConfigs { get; set; }
        public DbSet<RFQApprovalStatus> RfqApprovalStatuses { get; set; }
        public DbSet<RFQApprovalTransactions> RfqApprovalTransactions { get; set; }
        public DbSet<RFQDetails> RfqDetails { get; set; }
        public DbSet<RFQGeneration> RfqGenerations { get; set; }




        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasMany(u => u.Claims).WithOne().HasForeignKey(c => c.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<User>().HasMany(u => u.Roles).WithOne().HasForeignKey(r => r.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Role>().HasMany(r => r.Claims).WithOne().HasForeignKey(c => c.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Role>().HasMany(r => r.Users).WithOne().HasForeignKey(r => r.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);
        }
    }


    public class ContextDesignFactory : IDesignTimeDbContextFactory<EProcurementContext>
    {
        public EProcurementContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EProcurementContext>()
                .UseSqlServer("Server=tcp:174.142.93.40,1433;Initial Catalog=EProcurement;Persist Security Info=False;User ID=EProcurement;Password=EProcure$12345;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");

            return new EProcurementContext(optionsBuilder.Options);
        }
    }
}
