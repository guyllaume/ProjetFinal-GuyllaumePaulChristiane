using ProjetFinal_GuyllaumePaulChristiane.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProjetFinal_GuyllaumePaulChristiane.Models
{
    public class DVD
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre français est requis")]
        [Display(Name = "Titre français")]
        
        public string? TitreFrancais { get; set; }

        [Display(Name = "Titre original")]
        public string? TitreOriginal { get; set; }

        [Display(Name = "Année de sortie")]
        [Range(1900, 2100, ErrorMessage = "L'année doit être entre 1900 et 2100")]
        public int? AnneeSortie { get; set; }

        
        [Display(Name = "Catégorie")]
        public String? Categorie { get; set; }
        /*
        
        [Display(Name = "Catégorie")]
        public Categorie? Categorie { get; set; }
        */ 

        [Display(Name = "Dernière mise à jour effectuée le")]
        public DateTime? DerniereMiseAJour { get; set; }

        [Display(Name = "Dernière mise à jour effectuée par")]
        public string? DerniereMiseAJourPar { get; set; }

        [Display(Name = "Description des suppléments")]
        public string? DescriptionSupplements { get; set; }

        [Display(Name = "Durée (en minutes)")]
        [Range(1, 1000, ErrorMessage = "La durée doit être entre 1 et 1000 minutes")]
        public int? Duree { get; set; }

        [Display(Name = "DVD original")]
        public bool EstOriginal { get; set; }

        [Display(Name = "Format")]
        public string? Format { get; set; }

        [Display(Name = "Image de la pochette")]
        public string? ImagePochette { get; set; }

        [Display(Name = "Langue")]
        public string? Langue { get; set; }

        [Display(Name = "Nombre de disques")]
        [Range(1, 10, ErrorMessage = "Le nombre de disques doit être entre 1 et 10")]
        public int? NombreDisques { get; set; }

        [Display(Name = "Nom du producteur")]
        public string? NomProducteur { get; set; }

        [Display(Name = "Nom du réalisateur")]
        public string? NomRealisateur { get; set; }

        [Display(Name = "Noms des acteurs principaux")]
        public string? NomsActeursPrincipaux { get; set; }

        [Display(Name = "Résumé du film")]
        public string? ResumeFilm { get; set; }

        [Display(Name = "Sous-titres")]
        public string? SousTitres { get; set; }

        [Display(Name = "Utilisateur propriétaire")]
        public string? UtilisateurProprietaire { get; set; }

        [Display(Name = "Utilisateur emprunteur")]
        public string? UtilisateurEmprunteur { get; set; }

        [Display(Name = "Version étendue")]
        public bool VersionEtendue { get; set; }

        [Display(Name = "Visible à tous")]
        public bool VisibleATous { get; set; }

    }
}