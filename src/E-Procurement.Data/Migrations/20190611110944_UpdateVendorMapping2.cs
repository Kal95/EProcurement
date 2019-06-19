using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Procurement.Data.Migrations
{
    public partial class UpdateVendorMapping2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorMappings_VendorCategories_VendorCategoryId",
                table: "VendorMappings");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "VendorMappings");

            migrationBuilder.AlterColumn<int>(
                name: "VendorCategoryId",
                table: "VendorMappings",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorMappings_VendorCategories_VendorCategoryId",
                table: "VendorMappings",
                column: "VendorCategoryId",
                principalTable: "VendorCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendorMappings_VendorCategories_VendorCategoryId",
                table: "VendorMappings");

            migrationBuilder.AlterColumn<int>(
                name: "VendorCategoryId",
                table: "VendorMappings",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "VendorMappings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_VendorMappings_VendorCategories_VendorCategoryId",
                table: "VendorMappings",
                column: "VendorCategoryId",
                principalTable: "VendorCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
