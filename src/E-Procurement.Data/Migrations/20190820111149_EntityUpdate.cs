using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Procurement.Data.Migrations
{
    public partial class EntityUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Vendors",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RFQBody",
                table: "RfqDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RFQTitle",
                table: "RfqDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "POCost",
                table: "PoGenerations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "POPreamble",
                table: "PoGenerations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "POTerms",
                table: "PoGenerations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "POTitle",
                table: "PoGenerations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "POValidity",
                table: "PoGenerations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "POWarranty",
                table: "PoGenerations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "RFQBody",
                table: "RfqDetails");

            migrationBuilder.DropColumn(
                name: "RFQTitle",
                table: "RfqDetails");

            migrationBuilder.DropColumn(
                name: "POCost",
                table: "PoGenerations");

            migrationBuilder.DropColumn(
                name: "POPreamble",
                table: "PoGenerations");

            migrationBuilder.DropColumn(
                name: "POTerms",
                table: "PoGenerations");

            migrationBuilder.DropColumn(
                name: "POTitle",
                table: "PoGenerations");

            migrationBuilder.DropColumn(
                name: "POValidity",
                table: "PoGenerations");

            migrationBuilder.DropColumn(
                name: "POWarranty",
                table: "PoGenerations");
        }
    }
}
