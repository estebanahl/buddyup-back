using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace buddyUp.Migrations
{
    public partial class matchfunction3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Match",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userp1_id = table.Column<int>(type: "integer", nullable: false),
                    userp2_id = table.Column<int>(type: "integer", nullable: false),
                    isMatch = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Match", x => x.id);
                    table.ForeignKey(
                        name: "FK_Match_Profile_userp1_id",
                        column: x => x.userp1_id,
                        principalTable: "Profile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Match_Profile_userp2_id",
                        column: x => x.userp2_id,
                        principalTable: "Profile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Match_userp1_id",
                table: "Match",
                column: "userp1_id");

            migrationBuilder.CreateIndex(
                name: "IX_Match_userp2_id",
                table: "Match",
                column: "userp2_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Match");
        }
    }
}
