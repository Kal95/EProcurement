using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Procurement.Data.Migrations
{
    public partial class DroppedForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Banks_BankId",
                table: "Vendors");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Countries_CountryId",
                table: "Vendors");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_States_StateId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_BankId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_CountryId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_StateId",
                table: "Vendors");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Vendors_BankId",
                table: "Vendors",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_CountryId",
                table: "Vendors",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_StateId",
                table: "Vendors",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Banks_BankId",
                table: "Vendors",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Countries_CountryId",
                table: "Vendors",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_States_StateId",
                table: "Vendors",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
