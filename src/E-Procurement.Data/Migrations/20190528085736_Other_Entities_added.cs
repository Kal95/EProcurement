using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Procurement.Data.Migrations
{
    public partial class Other_Entities_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Vendors",
                newName: "WebsiteAddress");

            //migrationBuilder.RenameColumn(
            //    name: "Category",
            //    table: "VendorCategories",
            //    newName: "CategoryName");

            migrationBuilder.AddColumn<decimal>(
                name: "AatAmount",
                table: "Vendors",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "AatCurrency",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountNo",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankBranch",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "Vendors",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CacNo",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Vendors",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SortCode",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "Vendors",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TinNo",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VatNo",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VendorAddress",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VendorName",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VendorStatus",
                table: "Vendors",
                nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "SortCode",
            //    table: "Banks",
            //    nullable: true);

            migrationBuilder.CreateTable(
                name: "DnGenerations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastDateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    PoId = table.Column<int>(nullable: false),
                    DnFilePath = table.Column<string>(nullable: true),
                    DnFileBlob = table.Column<byte>(nullable: false),
                    DnRecievedBy = table.Column<string>(nullable: true),
                    DnUploadedDate = table.Column<DateTime>(nullable: false),
                    DnUploadedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DnGenerations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GrnGenerations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastDateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    POId = table.Column<int>(nullable: false),
                    GRNNo = table.Column<string>(nullable: true),
                    GRNFilePath = table.Column<string>(nullable: true),
                    GRNFileBlob = table.Column<byte>(nullable: false),
                    GRNUploadedDate = table.Column<DateTime>(nullable: false),
                    GRNUploadedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrnGenerations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PoGenerations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastDateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    RFQId = table.Column<int>(nullable: false),
                    PONumber = table.Column<string>(nullable: true),
                    VendorId = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTime>(nullable: false),
                    ActualDeliveryDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoGenerations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RfqApprovalConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastDateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    ApprovalLevel = table.Column<int>(nullable: false),
                    IsFinalLevel = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RfqApprovalConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RfqApprovalStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastDateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    RFQId = table.Column<int>(nullable: false),
                    CurrentApprovalLevel = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RfqApprovalStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RfqApprovalTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastDateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    VendorId = table.Column<int>(nullable: false),
                    RFQId = table.Column<int>(nullable: false),
                    ApprovalStatus = table.Column<string>(nullable: true),
                    ApprovedBy = table.Column<string>(nullable: true),
                    ApprovalLevel = table.Column<int>(nullable: false),
                    Comments = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RfqApprovalTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RfqDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastDateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    RFQId = table.Column<int>(nullable: false),
                    VendorId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    ItemName = table.Column<string>(nullable: true),
                    QuotedQuantity = table.Column<int>(nullable: false),
                    AgreedQuantity = table.Column<int>(nullable: false),
                    QuotedAmount = table.Column<decimal>(nullable: false),
                    AgreedAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RfqDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RfqGenerations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastDateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    ProjectId = table.Column<int>(nullable: false),
                    RequisitionId = table.Column<int>(nullable: false),
                    Reference = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    RFQStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RfqGenerations", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DnGenerations");

            migrationBuilder.DropTable(
                name: "GrnGenerations");

            migrationBuilder.DropTable(
                name: "PoGenerations");

            migrationBuilder.DropTable(
                name: "RfqApprovalConfigs");

            migrationBuilder.DropTable(
                name: "RfqApprovalStatuses");

            migrationBuilder.DropTable(
                name: "RfqApprovalTransactions");

            migrationBuilder.DropTable(
                name: "RfqDetails");

            migrationBuilder.DropTable(
                name: "RfqGenerations");

            migrationBuilder.DropColumn(
                name: "AatAmount",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "AatCurrency",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "AccountNo",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "BankBranch",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CacNo",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "SortCode",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "TinNo",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "VatNo",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "VendorAddress",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "VendorName",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "VendorStatus",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "SortCode",
                table: "Banks");

            migrationBuilder.RenameColumn(
                name: "WebsiteAddress",
                table: "Vendors",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "CategoryName",
                table: "VendorCategories",
                newName: "Category");
        }
    }
}
