using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Procurement.Data.Migrations
{
    public partial class VendorRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VendorCategoryId",
                table: "Vendors",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VendorCategoryId",
                table: "VendorMappings",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_VendorCategoryId",
                table: "Vendors",
                column: "VendorCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorMappings_VendorCategoryId",
                table: "VendorMappings",
                column: "VendorCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorMappings_VendorID",
                table: "VendorMappings",
                column: "VendorID");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorMappings_VendorCategories_VendorCategoryId",
                table: "VendorMappings",
                column: "VendorCategoryId",
                principalTable: "VendorCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorMappings_Vendors_VendorID",
                table: "VendorMappings",
                column: "VendorID",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_VendorCategories_VendorCategoryId",
                table: "Vendors",
                column: "VendorCategoryId",
                principalTable: "VendorCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorMappings_VendorCategories_VendorCategoryId",
                table: "VendorMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_VendorMappings_Vendors_VendorID",
                table: "VendorMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Banks_BankId",
                table: "Vendors");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Countries_CountryId",
                table: "Vendors");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_States_StateId",
                table: "Vendors");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_VendorCategories_VendorCategoryId",
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

            migrationBuilder.DropIndex(
                name: "IX_Vendors_VendorCategoryId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_VendorMappings_VendorCategoryId",
                table: "VendorMappings");

            migrationBuilder.DropIndex(
                name: "IX_VendorMappings_VendorID",
                table: "VendorMappings");

            migrationBuilder.DropColumn(
                name: "VendorCategoryId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "VendorCategoryId",
                table: "VendorMappings");
        }
    }
}
