using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Procurement.Data.Migrations
{
    public partial class POApproval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "PoGenerations",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "POApprovalTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VendorId = table.Column<int>(nullable: false),
                    RFQId = table.Column<int>(nullable: false),
                    POId = table.Column<int>(nullable: false),
                    ApprovalStatus = table.Column<string>(nullable: true),
                    ApprovedBy = table.Column<string>(nullable: true),
                    ApproverID = table.Column<int>(nullable: false),
                    ApprovalLevel = table.Column<int>(nullable: false),
                    Comments = table.Column<string>(nullable: true),
                    DateApproved = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POApprovalTransactions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "POApprovalTransactions");

            migrationBuilder.DropColumn(
                name: "Reference",
                table: "PoGenerations");
        }
    }
}
