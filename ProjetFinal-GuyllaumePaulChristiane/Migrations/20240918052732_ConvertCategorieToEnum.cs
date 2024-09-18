using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetFinal_GuyllaumePaulChristiane.Migrations
{
    /// <inheritdoc />
    public partial class ConvertCategorieToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Categorie",
                table: "DVDs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Categorie",
                table: "DVDs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
