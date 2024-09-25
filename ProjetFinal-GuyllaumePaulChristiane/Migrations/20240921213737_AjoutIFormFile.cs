using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetFinal_GuyllaumePaulChristiane.Migrations
{
    /// <inheritdoc />
    public partial class AjoutIFormFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePochette",
                table: "DVDs",
                newName: "CheminImage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CheminImage",
                table: "DVDs",
                newName: "ImagePochette");
        }
    }
}
