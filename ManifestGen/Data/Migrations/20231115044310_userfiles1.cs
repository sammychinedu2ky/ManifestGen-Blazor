using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManifestGen.Migrations
{
    /// <inheritdoc />
    public partial class userfiles1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFile_AspNetUsers_ApplicationUserId",
                table: "UserFile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFile",
                table: "UserFile");

            migrationBuilder.RenameTable(
                name: "UserFile",
                newName: "UserFiles");

            migrationBuilder.RenameIndex(
                name: "IX_UserFile_ApplicationUserId",
                table: "UserFiles",
                newName: "IX_UserFiles_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFiles",
                table: "UserFiles",
                column: "UserFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFiles_AspNetUsers_ApplicationUserId",
                table: "UserFiles",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFiles_AspNetUsers_ApplicationUserId",
                table: "UserFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFiles",
                table: "UserFiles");

            migrationBuilder.RenameTable(
                name: "UserFiles",
                newName: "UserFile");

            migrationBuilder.RenameIndex(
                name: "IX_UserFiles_ApplicationUserId",
                table: "UserFile",
                newName: "IX_UserFile_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFile",
                table: "UserFile",
                column: "UserFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFile_AspNetUsers_ApplicationUserId",
                table: "UserFile",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
