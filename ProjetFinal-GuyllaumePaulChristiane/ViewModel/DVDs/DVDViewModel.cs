using ProjetFinal_GuyllaumePaulChristiane.Models;

namespace ProjetFinal_GuyllaumePaulChristiane.ViewModel.DVDs
{
    public class DVDViewModel
    {
        public int TotalPages { get; set; }
        public int currentPage { get; set; }
        public bool isResearched { get; set; }
        public IEnumerable<DVD> DVDs { get; set; }
    }
}
