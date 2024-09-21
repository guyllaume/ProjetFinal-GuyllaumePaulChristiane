using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProjetFinal_GuyllaumePaulChristiane.Models
{
    public class User : IdentityUser
    {
        [Display(Name = "Recevoir un Courriel quand un DVD est créé ?")]
        public bool courrielOnDVDCreate { get; set; }
        [Display(Name = "Recevoir un Courriel quand un DVD est supprimé ?")]
        public bool courrielOnDVDDelete { get; set; }
        [Display(Name = "Recevoir un Courriel quand un DVD est approprié par quelqu'un d'autre ?")]
        public bool courrielOnAppropriation { get; set; }
        [Display(Name = "Nombre de DVD par page ?")]
        [Range(6,99)]
        public int nbDVDParPage { get; set; }
    }
}
