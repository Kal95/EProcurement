using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Procurement.Data.Migrations
{
    public partial class UpdateVendorMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "VendorMappings");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "VendorMappings");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "VendorMappings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "VendorMappings");

            migrationBuilder.DropColumn(
                name: "LastDateUpdated",
                table: "VendorMappings");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "VendorMappings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "VendorMappings",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "VendorMappings",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "VendorMappings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "VendorMappings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastDateUpdated",
                table: "VendorMappings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "VendorMappings",
                nullable: true);
        }
    }
}
