using Microsoft.AspNetCore.Identity.UI.Services;

namespace ProjetFinal_GuyllaumePaulChristiane.Tasks
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Ici, vous pouvez ajouter la logique pour envoyer des emails.
            // Pour l'instant, on simule l'envoi d'e-mails.
            return Task.CompletedTask;
        }
    }
}
