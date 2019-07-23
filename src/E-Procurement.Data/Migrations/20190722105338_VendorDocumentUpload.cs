using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Procurement.Data.Migrations
{
    public partial class VendorDocumentUpload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankRefFilePath",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "COVFilePath",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MOAFilePath",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NOSFilePath",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PODFilePath",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "POSFilePath",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefFilePath",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxFilePath",
                table: "Vendors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankRefFilePath",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "COVFilePath",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "MOAFilePath",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "NOSFilePath",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "PODFilePath",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "POSFilePath",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "RefFilePath",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "TaxFilePath",
                table: "Vendors");
        }
    }
}
