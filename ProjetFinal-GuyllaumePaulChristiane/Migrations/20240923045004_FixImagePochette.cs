using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetFinal_GuyllaumePaulChristiane.Migrations
{
    /// <inheritdoc />
    public partial class FixImagePochette : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePochette",
                table: "DVDs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePochette",
                table: "DVDs");
        }
    }
}
