using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoExchange.Migrations
{
    public partial class AddExchangeCoin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExchangeCoins",
                columns: table => new
                {
                    CoinId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeCoins", x => x.CoinId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeCoins");
        }
    }
}
