using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Procurement.Data.Migrations
{
    public partial class VendorEvaluation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           


            migrationBuilder.AlterColumn<int>(
                name: "ItemCategoryId",
                table: "Items",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "VendorEvaluations",
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
                    VendorId = table.Column<string>(nullable: true),
                    VendorName = table.Column<string>(nullable: true),
                    BestPrice = table.Column<string>(nullable: true),
                    ProductAvailability = table.Column<string>(nullable: true),
                    ProductQuality = table.Column<string>(nullable: true),
                    DeliveryTimeFrame = table.Column<string>(nullable: true),
                    CreditFacility = table.Column<string>(nullable: true),
                    WarrantySupport = table.Column<string>(nullable: true),
                    CustomerCare = table.Column<string>(nullable: true),
                    Others = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorEvaluations", x => x.Id);
                });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Vendors_BankId",
            //    table: "Vendors",
            //    column: "BankId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Vendors_CountryId",
            //    table: "Vendors",
            //    column: "CountryId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Vendors_StateId",
            //    table: "Vendors",
            //    column: "StateId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_VendorMappings_Vendors_VendorID",
            //    table: "VendorMappings",
            //    column: "VendorID",
            //    principalTable: "Vendors",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Vendors_Banks_BankId",
            //    table: "Vendors",
            //    column: "BankId",
            //    principalTable: "Banks",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Vendors_Countries_CountryId",
            //    table: "Vendors",
            //    column: "CountryId",
            //    principalTable: "Countries",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Vendors_States_StateId",
            //    table: "Vendors",
            //    column: "StateId",
            //    principalTable: "States",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropTable(
                name: "VendorEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_BankId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_CountryId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_StateId",
                table: "Vendors");

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
