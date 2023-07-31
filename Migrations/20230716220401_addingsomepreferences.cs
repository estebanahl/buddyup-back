using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace buddyUp.Migrations
{
    public partial class addingsomepreferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "maximun_age",
                table: "Profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "maximun_distance",
                table: "Profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "minimun_age",
                table: "Profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "minimun_distance",
                table: "Profile",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "maximun_age",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "maximun_distance",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "minimun_age",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "minimun_distance",
                table: "Profile");
        }
    }
}
