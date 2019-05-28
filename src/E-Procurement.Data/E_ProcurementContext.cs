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
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionRole> PermissionRoles { get; set; }

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
