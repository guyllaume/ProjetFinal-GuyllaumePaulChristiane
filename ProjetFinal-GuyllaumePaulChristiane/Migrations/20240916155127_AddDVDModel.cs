using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetFinal_GuyllaumePaulChristiane.Migrations
{
    /// <inheritdoc />
    public partial class AddDVDModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DVDs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitreFrancais = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitreOriginal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnneeSortie = table.Column<int>(type: "int", nullable: true),
                    Categorie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DerniereMiseAJour = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DerniereMiseAJourPar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionSupplements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duree = table.Column<int>(type: "int", nullable: true),
                    EstOriginal = table.Column<bool>(type: "bit", nullable: false),
                    Format = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePochette = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Langue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreDisques = table.Column<int>(type: "int", nullable: true),
                    NomProducteur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomRealisateur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomsActeursPrincipaux = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResumeFilm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SousTitres = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UtilisateurProprietaire = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UtilisateurEmprunteur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VersionEtendue = table.Column<bool>(type: "bit", nullable: false),
                    VisibleATous = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DVDs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DVDs");
        }
    }
}
