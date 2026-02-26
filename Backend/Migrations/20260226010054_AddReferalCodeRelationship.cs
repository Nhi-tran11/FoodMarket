using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddReferalCodeRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_ReferalCodes_ReferredByCodeId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferalCodes_Customers_ReferrerId",
                table: "ReferalCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferalCodes_Customers_UsedByUserId",
                table: "ReferalCodes");

            migrationBuilder.DropIndex(
                name: "IX_Customers_ReferredByCodeId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "NewUserDiscountId",
                table: "ReferalCodes");

            migrationBuilder.DropColumn(
                name: "ReferredByCodeId",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "UsedByUserId",
                table: "ReferalCodes",
                newName: "ReceivedByUserId");

            migrationBuilder.RenameColumn(
                name: "ReferrerId",
                table: "ReferalCodes",
                newName: "RefererId");

            migrationBuilder.RenameIndex(
                name: "IX_ReferalCodes_UsedByUserId",
                table: "ReferalCodes",
                newName: "IX_ReferalCodes_ReceivedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ReferalCodes_ReferrerId",
                table: "ReferalCodes",
                newName: "IX_ReferalCodes_RefererId");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "ReferalCodes",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercentage",
                table: "ReferalCodes",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "ReferalCodes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReferalCodes_Customers_ReceivedByUserId",
                table: "ReferalCodes",
                column: "ReceivedByUserId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReferalCodes_Customers_RefererId",
                table: "ReferalCodes",
                column: "RefererId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReferalCodes_Customers_ReceivedByUserId",
                table: "ReferalCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_ReferalCodes_Customers_RefererId",
                table: "ReferalCodes");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "ReferalCodes");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "ReferalCodes");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "ReferalCodes");

            migrationBuilder.RenameColumn(
                name: "RefererId",
                table: "ReferalCodes",
                newName: "ReferrerId");

            migrationBuilder.RenameColumn(
                name: "ReceivedByUserId",
                table: "ReferalCodes",
                newName: "UsedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ReferalCodes_RefererId",
                table: "ReferalCodes",
                newName: "IX_ReferalCodes_ReferrerId");

            migrationBuilder.RenameIndex(
                name: "IX_ReferalCodes_ReceivedByUserId",
                table: "ReferalCodes",
                newName: "IX_ReferalCodes_UsedByUserId");

            migrationBuilder.AddColumn<int>(
                name: "NewUserDiscountId",
                table: "ReferalCodes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReferredByCodeId",
                table: "Customers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ReferredByCodeId",
                table: "Customers",
                column: "ReferredByCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_ReferalCodes_ReferredByCodeId",
                table: "Customers",
                column: "ReferredByCodeId",
                principalTable: "ReferalCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ReferalCodes_Customers_ReferrerId",
                table: "ReferalCodes",
                column: "ReferrerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReferalCodes_Customers_UsedByUserId",
                table: "ReferalCodes",
                column: "UsedByUserId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
