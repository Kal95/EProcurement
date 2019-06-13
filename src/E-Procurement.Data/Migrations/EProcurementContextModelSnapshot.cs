﻿// <auto-generated />
using System;
using E_Procurement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace E_Procurement.Data.Migrations
{
    [DbContext(typeof(EProcurementContext))]
    partial class EProcurementContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("E_Procurement.Data.Entity.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateCreated");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("LastDateUpdated");

                    b.Property<string>("Name");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("E_Procurement.Data.Entity.PermissionRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateCreated");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("LastDateUpdated");

                    b.Property<int>("PermissionId");

                    b.Property<int>("RoleId");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.ToTable("DnGenerations");
                });

            modelBuilder.Entity("E_Procurement.Data.Entity.GRNGeneration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateCreated");

                    b.Property<byte>("GRNFileBlob");

                    b.Property<string>("GRNFilePath");

                    b.Property<string>("GRNNo");

                    b.Property<string>("GRNUploadedBy");

                    b.Property<DateTime>("GRNUploadedDate");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("LastDateUpdated");

                    b.Property<int>("POId");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.ToTable("GrnGenerations");
                });

            modelBuilder.Entity("E_Procurement.Data.Entity.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ItemCategoryId");

                    b.Property<string>("ItemName");

                    b.HasKey("Id");

                    b.HasIndex("ItemCategoryId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("E_Procurement.Data.Entity.ItemCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CategoryName");

                    b.HasKey("Id");

                    b.ToTable("ItemCategories");
                });

            modelBuilder.Entity("E_Procurement.Data.Entity.POGeneration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("ActualDeliveryDate");

                    b.Property<decimal>("Amount");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("ExpectedDeliveryDate");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("LastDateUpdated");

                    b.Property<string>("PONumber");

                    b.Property<int>("RFQId");

                    b.Property<string>("UpdatedBy");

                    b.Property<int>("VendorId");

                    b.HasKey("Id");

                    b.ToTable("PoGenerations");
                });

            modelBuilder.Entity("E_Procurement.Data.Entity.RFQApprovalConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ApprovalLevel");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Email");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsFinalLevel");

                    b.Property<DateTime?>("LastDateUpdated");

                    b.Property<string>("UpdatedBy");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("RfqApprovalConfigs");
                });

            modelBuilder.Entity("E_Procurement.Data.Entity.RFQApprovalStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<int>("CurrentApprovalLevel");

                    b.Property<DateTime>("DateCreated");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("LastDateUpdated");

                    b.Property<int>("RFQId");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.ToTable("RfqApprovalStatuses");
                });

            modelBuilder.Entity("E_Procurement.Data.Entity.RFQApprovalTransactions", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ApprovalLevel");

                    b.Property<string>("ApprovalStatus");

                    b.Property<string>("ApprovedBy");

                    b.Property<int>("Comments");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateCreated");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("LastDateUpdated");

                    b.Property<int>("RFQId");

                    b.Property<string>("UpdatedBy");

                    b.Property<int>("VendorId");

                    b.HasKey("Id");

                    b.ToTable("RfqApprovalTransactions");
                });

            modelBuilder.Entity("E_Procurement.Data.Entity.RFQDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("AgreedAmount");

                    b.Property<int>("AgreedQuantity");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateCreated");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("ItemId");

                    b.Property<string>("ItemName");

                    b.Property<DateTime?>("LastDateUpdated");

                    b.Property<decimal>("QuotedAmount");

                    b.Property<int>("QuotedQuantity");

                    b.Property<int>("RFQId");

                    b.Property<string>("UpdatedBy");

                    b.Property<int>("VendorId");

                    b.HasKey("Id");

                    b.ToTable("RfqDetails");
                });

            modelBuilder.Entity("E_Procurement.Data.Entity.RFQGeneration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Description");

                    b.Property<DateTime>("EndDate");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("LastDateUpdated");

                    b.Property<int>("ProjectId");

                    b.Property<string>("RFQStatus");

                    b.Property<string>("Reference");

                    b.Property<int>("RequisitionId");

                    b.Property<DateTime>("StartDate");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.ToTable("RfqGenerations");
                });

            modelBuilder.Entity("E_Procurement.Data.Entity.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("E_Procurement.Data.Entity.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Department");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("MiddleName");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("Unit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("E_Procurement.Data.Entity.Vendor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateCreated");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("LastDateUpdated");

                    b.Property<string>("Name");

                    b.Property<string>("UpdatedBy");

                    b.HasKey("Id");

                    b.ToTable("Vendors");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("RoleId");

                    b.Property<int?>("RoleId1");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("RoleId1");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("UserId");

                    b.Property<int?>("UserId1");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("UserId1");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<int>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.Property<int?>("RoleId1");

                    b.Property<int?>("UserId1");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("RoleId1");

                    b.HasIndex("UserId1");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("E_Procurement.Data.Entity.Item", b =>
                {
                    b.HasOne("E_Procurement.Data.Entity.ItemCategory")
                        .WithMany("Items")
                        .HasForeignKey("ItemCategoryId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("E_Procurement.Data.Entity.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("E_Procurement.Data.Entity.Role")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId1");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("E_Procurement.Data.Entity.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("E_Procurement.Data.Entity.User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId1");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("E_Procurement.Data.Entity.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.HasOne("E_Procurement.Data.Entity.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("E_Procurement.Data.Entity.Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId1");

                    b.HasOne("E_Procurement.Data.Entity.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("E_Procurement.Data.Entity.User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId1");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("E_Procurement.Data.Entity.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
