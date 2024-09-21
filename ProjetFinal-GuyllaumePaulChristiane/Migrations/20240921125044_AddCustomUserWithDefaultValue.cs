using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetFinal_GuyllaumePaulChristiane.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomUserWithDefaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "courrielOnAppropriation",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "courrielOnDVDCreate",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "courrielOnDVDDelete",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "nbDVDParPage",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 12);

            migrationBuilder.AddCheckConstraint(
                name: "CK_DVD_per_pages",
                table: "AspNetUsers",
                sql: "nbDVDParPage >= 6 AND nbDVDParPage <= 99");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_DVD_per_pages",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "courrielOnAppropriation",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "courrielOnDVDCreate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "courrielOnDVDDelete",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "nbDVDParPage",
                table: "AspNetUsers");
        }
    }
}
