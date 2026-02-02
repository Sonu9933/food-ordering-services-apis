using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class orderentitychanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Restaurants_RestaurantID1",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_RestaurantID1",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RestaurantID1",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RestaurantID1",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_RestaurantID1",
                table: "Orders",
                column: "RestaurantID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Restaurants_RestaurantID1",
                table: "Orders",
                column: "RestaurantID1",
                principalTable: "Restaurants",
                principalColumn: "RestaurantID");
        }
    }
}
