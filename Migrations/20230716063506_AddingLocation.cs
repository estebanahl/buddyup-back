using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace buddyUp.Migrations
{
    public partial class AddingLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "aprox_location",
                table: "Profile",
                type: "text",
                nullable: true,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "aprox_location",
                table: "Profile");
        }
    }
}
