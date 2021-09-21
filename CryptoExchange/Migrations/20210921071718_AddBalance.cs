using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoExchange.Migrations
{
    public partial class AddBalance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBuy",
                table: "Transactions");

            migrationBuilder.AddColumn<double>(
                name: "Balance",
                table: "Users",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "IsBuy",
                table: "Transactions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
