using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Procurement.Data.Migrations
{
    public partial class EntitiesUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Vendors",
                newName: "WebsiteAddress");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "VendorCategories",
                newName: "CategoryName");

            migrationBuilder.AddColumn<decimal>(
                name: "AATAmount",
                table: "Vendors",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "AATCurrency",
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
                name: "CACNo",
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
                name: "TINNo",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VATNo",
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

            migrationBuilder.AddColumn<string>(
                name: "SortCode",
                table: "Banks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AATAmount",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "AATCurrency",
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
                name: "CACNo",
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
                name: "TINNo",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "VATNo",
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
