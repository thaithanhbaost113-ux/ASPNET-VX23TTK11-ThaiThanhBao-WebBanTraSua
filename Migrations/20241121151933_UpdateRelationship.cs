using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    public partial class UpdateRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EMAIL",
                table: "CART");

            migrationBuilder.DropColumn(
                name: "EMAIL",
                table: "BILL");

            migrationBuilder.RenameColumn(
                name: "NAMEProduct",
                table: "Product",
                newName: "NAMEPRODUCT");

            migrationBuilder.AddColumn<string>(
                name: "USERID",
                table: "CART",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "USERID",
                table: "BILL",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CART_USERID",
                table: "CART",
                column: "USERID");

            migrationBuilder.CreateIndex(
                name: "IX_BILL_USERID",
                table: "BILL",
                column: "USERID");

            migrationBuilder.AddForeignKey(
                name: "FK_BILL_APPLICATION_USER",
                table: "BILL",
                column: "USERID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CART_APPLICATION_USER",
                table: "CART",
                column: "USERID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BILL_APPLICATION_USER",
                table: "BILL");

            migrationBuilder.DropForeignKey(
                name: "FK_CART_APPLICATION_USER",
                table: "CART");

            migrationBuilder.DropIndex(
                name: "IX_CART_USERID",
                table: "CART");

            migrationBuilder.DropIndex(
                name: "IX_BILL_USERID",
                table: "BILL");

            migrationBuilder.DropColumn(
                name: "USERID",
                table: "CART");

            migrationBuilder.DropColumn(
                name: "USERID",
                table: "BILL");

            migrationBuilder.RenameColumn(
                name: "NAMEPRODUCT",
                table: "Product",
                newName: "NAMEProduct");

            migrationBuilder.AddColumn<string>(
                name: "EMAIL",
                table: "CART",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EMAIL",
                table: "BILL",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
