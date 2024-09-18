using ProjetFinal_GuyllaumePaulChristiane.Models;

namespace ProjetFinal_GuyllaumePaulChristiane.ViewModel.DVDs
{
    public class SelectedUserViewModel
    {
        public string SelectedUserId { get; set; }
        public string UserName { get; set; }
        public int TotalPages { get; set; }
        public int currentPage { get; set; }
        public IEnumerable<DVD> DVDs { get; set; }
    }
}
