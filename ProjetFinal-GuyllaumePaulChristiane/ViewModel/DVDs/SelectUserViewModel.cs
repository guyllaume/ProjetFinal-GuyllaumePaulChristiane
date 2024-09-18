using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetFinal_GuyllaumePaulChristiane.Models;

namespace ProjetFinal_GuyllaumePaulChristiane.ViewModel.DVDs
{
    public class SelectUserViewModel
    {
        public string SelectedUserId { get; set; }
        public List<SelectListItem> Users { get; set; } 
    }
}
