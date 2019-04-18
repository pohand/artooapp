using Microsoft.EntityFrameworkCore.Migrations;

namespace ArtooApp.Migrations
{
    public partial class Tenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "TechManagers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "PassionBrands",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Mistakes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Inspections",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Factories",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Emails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "EmailRules",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "EmailRuleDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TechManagers");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PassionBrands");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Mistakes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Inspections");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Factories");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "EmailRules");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "EmailRuleDetails");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AspNetUsers");
        }
    }
}
