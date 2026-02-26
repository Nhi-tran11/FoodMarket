using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddUsedReferalCodeToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsedReferalCodeId",
                table: "Orders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UsedReferalCodeId",
                table: "Orders",
                column: "UsedReferalCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ReferalCodes_UsedReferalCodeId",
                table: "Orders",
                column: "UsedReferalCodeId",
                principalTable: "ReferalCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ReferalCodes_UsedReferalCodeId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UsedReferalCodeId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UsedReferalCodeId",
                table: "Orders");
        }
    }
}
