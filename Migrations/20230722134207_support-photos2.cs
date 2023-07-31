using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace buddyUp.Migrations
{
    public partial class supportphotos2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photo_Profile_UserId",
                table: "Photo");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Photo",
                newName: "UserProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Photo_UserId",
                table: "Photo",
                newName: "IX_Photo_UserProfileId");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Photo",
                type: "text",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea");

            migrationBuilder.AddForeignKey(
                name: "FK_Photo_Profile_UserProfileId",
                table: "Photo",
                column: "UserProfileId",
                principalTable: "Profile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photo_Profile_UserProfileId",
                table: "Photo");

            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                table: "Photo",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Photo_UserProfileId",
                table: "Photo",
                newName: "IX_Photo_UserId");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Image",
                table: "Photo",
                type: "bytea",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Photo_Profile_UserId",
                table: "Photo",
                column: "UserId",
                principalTable: "Profile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
