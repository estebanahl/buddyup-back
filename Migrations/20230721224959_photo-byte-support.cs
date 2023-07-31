using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace buddyUp.Migrations
{
    public partial class photobytesupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Photo");
       

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Photo",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {  
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Photo");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Photo",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
