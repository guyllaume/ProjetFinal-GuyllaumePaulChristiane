using ProjetFinal_GuyllaumePaulChristiane.Models;

namespace ProjetFinal_GuyllaumePaulChristiane.ViewModel.DVDs
{
    public class ContactViewModel
    {
        public string? StatusMessage { get; set; }
        public DVD? dvd { get; set; }
        public int? dvdId { get; set; }
        public string? username { get; set; }
        public string? message { get; set; }
        public string? sujet { get; set; }
        public List<string>? userAContacter { get; set; }
        public List<string>? userContactable{ get; set; }
    }
}
