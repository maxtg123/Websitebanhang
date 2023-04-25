using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.Migrations
{
    public partial class _44444 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeneralInfo",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OtherInfo",
                table: "Products");

            migrationBuilder.AddColumn<bool>(
                name: "PaymentMethod",
                table: "Invoices",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Invoices");

            migrationBuilder.AddColumn<string>(
                name: "GeneralInfo",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherInfo",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
