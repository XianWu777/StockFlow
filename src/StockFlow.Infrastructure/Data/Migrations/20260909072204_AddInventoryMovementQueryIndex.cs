using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryMovementQueryIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_ProductId_CreatedAt",
                table: "InventoryMovements",
                columns: new[] { "ProductId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryMovements_ProductId_CreatedAt",
                table: "InventoryMovements");
        }
    }
}
