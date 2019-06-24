using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Procurement.Data.Migrations
{
    public partial class VendorCategoryMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_ItemCategories_ItemCategoryId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_ItemCategoryId",
                table: "Items");

            migrationBuilder.AlterColumn<int>(
                name: "ItemCategoryId",
                table: "Items",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "VendorMappings",
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
                    VendorID = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorMappings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorMappings");

            migrationBuilder.AlterColumn<int>(
                name: "ItemCategoryId",
                table: "Items",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemCategoryId",
                table: "Items",
                column: "ItemCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ItemCategories_ItemCategoryId",
                table: "Items",
                column: "ItemCategoryId",
                principalTable: "ItemCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
