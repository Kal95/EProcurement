using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Procurement.Data.Migrations
{
    public partial class ItemDetailsPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AgreedPrice",
                table: "RfqDetails",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "QuotedPrice",
                table: "RfqDetails",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgreedPrice",
                table: "RfqDetails");

            migrationBuilder.DropColumn(
                name: "QuotedPrice",
                table: "RfqDetails");
        }
    }
}
