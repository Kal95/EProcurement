using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Procurement.Data.Migrations
{
    public partial class UpdateVendorMapping3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorMappings_VendorCategories_VendorCategoryId",
                table: "VendorMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_VendorCategories_VendorCategoryId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_VendorCategoryId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_VendorMappings_VendorCategoryId",
                table: "VendorMappings");

            migrationBuilder.DropColumn(
                name: "VendorCategoryId",
                table: "Vendors");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VendorCategoryId",
                table: "Vendors",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_VendorCategoryId",
                table: "Vendors",
                column: "VendorCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorMappings_VendorCategoryId",
                table: "VendorMappings",
                column: "VendorCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_VendorMappings_VendorCategories_VendorCategoryId",
                table: "VendorMappings",
                column: "VendorCategoryId",
                principalTable: "VendorCategories",
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
    }
}
