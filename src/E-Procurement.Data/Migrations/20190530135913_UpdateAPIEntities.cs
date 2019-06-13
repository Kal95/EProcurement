using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Procurement.Data.Migrations
{
    public partial class UpdateAPIEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "LastDateUpdated",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ItemCategories");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "ItemCategories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ItemCategories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ItemCategories");

            migrationBuilder.DropColumn(
                name: "LastDateUpdated",
                table: "ItemCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ItemCategories");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Items",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Items",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Items",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastDateUpdated",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Items",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ItemCategories",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "ItemCategories",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ItemCategories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ItemCategories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastDateUpdated",
                table: "ItemCategories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ItemCategories",
                nullable: true);
        }
    }
}
