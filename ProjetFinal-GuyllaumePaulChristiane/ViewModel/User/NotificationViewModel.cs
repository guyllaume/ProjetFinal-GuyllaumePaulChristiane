using System.ComponentModel.DataAnnotations;

namespace ProjetFinal_GuyllaumePaulChristiane.ViewModel.User
{
    public class NotificationViewModel
    {
        public bool courrielOnDVDCreate { get; set; }
        public bool courrielOnDVDDelete { get; set; }
        public bool courrielOnAppropriation { get; set; }
    }
}
