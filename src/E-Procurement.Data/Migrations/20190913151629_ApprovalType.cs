using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Procurement.Data.Migrations
{
    public partial class ApprovalType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlteredBy",
                table: "RfqGenerations",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAltered",
                table: "RfqGenerations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ApprovalTypeId",
                table: "RfqApprovalConfigs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ApprovalTypes",
                columns: table => new
                {
                    ApprovalTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApprovalTypeName = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastDateUpdated = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalTypes", x => x.ApprovalTypeId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalTypes");

            migrationBuilder.DropColumn(
                name: "AlteredBy",
                table: "RfqGenerations");

            migrationBuilder.DropColumn(
                name: "DateAltered",
                table: "RfqGenerations");

            migrationBuilder.DropColumn(
                name: "ApprovalTypeId",
                table: "RfqApprovalConfigs");
        }
    }
}
