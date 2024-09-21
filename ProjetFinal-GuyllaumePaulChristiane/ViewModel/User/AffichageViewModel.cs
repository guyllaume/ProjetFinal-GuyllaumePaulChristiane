using System.ComponentModel.DataAnnotations;

namespace ProjetFinal_GuyllaumePaulChristiane.ViewModel.User
{
    public class AffichageViewModel
    {
        [Range(6, 99)]
        public int nbDVDParPage { get; set; }
    }
}
