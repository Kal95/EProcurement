using E_Procurement.Data.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace E_Procurement.Data
{
    public class EProcurementContext: IdentityDbContext<User,Role,int>
    {

        private readonly IHttpContextAccessor _contextAccessor;
        public EProcurementContext(DbContextOptions<EProcurementContext> options, IHttpContextAccessor contextAccessor) :base(options)
        {
            _contextAccessor = contextAccessor;
        }

        public EProcurementContext(DbContextOptions options) : base(options)
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
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<VendorMapping> VendorMappings { get; set; }
        public DbSet<VendorEvaluation> VendorEvaluations { get; set; }
        public DbSet<EvaluationPeriodConfig> EvaluationPeriodConfigs { get; set; }
        public DbSet<UserToCategoryConfig> UserToCategoryConfigs { get; set; }
        public DbSet<ApprovalType> ApprovalTypes { get; set; }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionRole> PermissionRoles { get; set; }
        public DbSet<EmailSentLog> EmailSentLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasMany(u => u.Claims).WithOne().HasForeignKey(c => c.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<User>().HasMany(u => u.Roles).WithOne().HasForeignKey(r => r.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Role>().HasMany(r => r.Claims).WithOne().HasForeignKey(c => c.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Role>().HasMany(r => r.Users).WithOne().HasForeignKey(r => r.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);
        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            int saved = 0;
            var currentUser = _contextAccessor.HttpContext.User.Identity.Name;
            var currentDate = DateTime.Now;
            try
            {
                foreach (var entry in ChangeTracker.Entries<BaseEntity.Entity>()
               .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.DateCreated = currentDate;
                        entry.Entity.CreatedBy = currentUser;
                        //entry.Entity.LastDateUpdated = currentDate;
                        //entry.Entity.UpdatedBy = currentUser;
                    }
                    if (entry.State == EntityState.Modified)
                    {
                        entry.Entity.LastDateUpdated = currentDate;
                        entry.Entity.UpdatedBy = currentUser;
                        //if (entry.Entity.IsDeleted == true && entry.Entity. == null)
                        //{
                        //    entry.Entity.DateDeleted = currentDate;
                        //    entry.Entity.DeletedBy = currentUser;
                        //}
                    }
                }
                saved = await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception Ex)
            {
                //Log.Error(Ex, "An error has occured in SaveChanges");
                //Log.Error(Ex.InnerException, "An error has occured in SaveChanges InnerException");
                //Log.Error(Ex.StackTrace, "An error has occured in SaveChanges StackTrace");
            }

            return saved;
            //   return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            int saved = 0;
            var currentUser = _contextAccessor.HttpContext.User.Identity.Name;
            var currentDate = DateTime.Now;
            try
            {
                foreach (var entry in ChangeTracker.Entries<BaseEntity.Entity>()
               .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
                {
                    if (entry.State == EntityState.Added)
                    {
                        entry.Entity.DateCreated = currentDate;
                        entry.Entity.CreatedBy = currentUser;
                        //entry.Entity.LastDateUpdated = currentDate;
                        //entry.Entity.UpdatedBy = currentUser;
                    }
                    if (entry.State == EntityState.Modified)
                    {
                        entry.Entity.LastDateUpdated = currentDate;
                        entry.Entity.UpdatedBy = currentUser;
                        //if (entry.Entity.IsDeleted == true && entry.Entity.DateDeleted == null)
                        //{
                        //    entry.Entity.DateDeleted = currentDate;
                        //    entry.Entity.DeletedBy = currentUser;
                        //}
                    }
                }
                saved = base.SaveChanges();
            }
            catch (Exception Ex)
            {
                //Log.Error(Ex, "An error has occured in SaveChanges");
                //Log.Error(Ex.InnerException, "An error has occured in SaveChanges InnerException");
                //Log.Error(Ex.StackTrace, "An error has occured in SaveChanges StackTrace");
                throw Ex;
            }

            return saved;
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
  