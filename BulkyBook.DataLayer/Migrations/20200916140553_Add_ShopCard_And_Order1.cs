using Microsoft.EntityFrameworkCore.Migrations;

namespace BulkyBook.DataLayer.Migrations
{
    public partial class Add_ShopCard_And_Order1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_AspNetUsers_ApplicationUserId",
                table: "OrderHeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_AspNetUsers_ApplicationUserId",
                table: "ShoppingCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_Products_ProductId",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCarts_ApplicationUserId",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCarts_ProductId",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeaders_ApplicationUserId",
                table: "OrderHeaders");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "OrderDetails");

            migrationBuilder.AddColumn<string>(
                name: "MyUserId",
                table: "ShoppingCarts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MyUserId",
                table: "OrderHeaders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_MyUserId",
                table: "ShoppingCarts",
                column: "MyUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_ProId",
                table: "ShoppingCarts",
                column: "ProId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_MyUserId",
                table: "OrderHeaders",
                column: "MyUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProId",
                table: "OrderDetails",
                column: "ProId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProId",
                table: "OrderDetails",
                column: "ProId",
                principalTable: "Products",
                principalColumn: "ProId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_AspNetUsers_MyUserId",
                table: "OrderHeaders",
                column: "MyUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_AspNetUsers_MyUserId",
                table: "ShoppingCarts",
                column: "MyUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_Products_ProId",
                table: "ShoppingCarts",
                column: "ProId",
                principalTable: "Products",
                principalColumn: "ProId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_AspNetUsers_MyUserId",
                table: "OrderHeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_AspNetUsers_MyUserId",
                table: "ShoppingCarts");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCarts_Products_ProId",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCarts_MyUserId",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingCarts_ProId",
                table: "ShoppingCarts");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeaders_MyUserId",
                table: "OrderHeaders");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ProId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "MyUserId",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "MyUserId",
                table: "OrderHeaders");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "ShoppingCarts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "ShoppingCarts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "OrderHeaders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "OrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_ApplicationUserId",
                table: "ShoppingCarts",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCarts_ProductId",
                table: "ShoppingCarts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_ApplicationUserId",
                table: "OrderHeaders",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_AspNetUsers_ApplicationUserId",
                table: "OrderHeaders",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_AspNetUsers_ApplicationUserId",
                table: "ShoppingCarts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCarts_Products_ProductId",
                table: "ShoppingCarts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
