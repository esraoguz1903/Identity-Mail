using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityMail.Web.Migrations
{
    /// <inheritdoc />
    public partial class addrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "UserMessages");

            migrationBuilder.DropColumn(
                name: "MessageCount",
                table: "Categories");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "UserMessages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_CategoryId",
                table: "UserMessages",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMessages_Categories_CategoryId",
                table: "UserMessages",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMessages_Categories_CategoryId",
                table: "UserMessages");

            migrationBuilder.DropIndex(
                name: "IX_UserMessages_CategoryId",
                table: "UserMessages");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "UserMessages");

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "UserMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MessageCount",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
