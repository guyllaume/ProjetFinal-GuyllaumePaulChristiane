using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MailKit.Net.Smtp;
using ProjetFinal_GuyllaumePaulChristiane.Models;
using Azure.Core;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace ProjetFinal_GuyllaumePaulChristiane.Tasks
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpServer = "smtp.gmail.com"; // Replace with your SMTP server
        private readonly int _smtpPort = 587; // Replace with your SMTP port
        private readonly string _smtpUser = "noreply.guyllaume@gmail.com"; // Replace with your SMTP username
        private readonly string _smtpPass = "dcxdxviuhhnawlec"; // Replace with your SMTP password

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("DVDProject", _smtpUser));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlMessage };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpUser, _smtpPass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
        public async Task SendEmailAsync(string emailSender, string emailReceiver, string subject, string htmlMessage, DVD dvd)
        {
            // Build your HTML message with DVD details
            var dvdDetails = $"<h2>DVD Details</h2>" +
                             $"<p><strong>Titre Francais:</strong> {dvd.TitreFrancais}</p>" +
                             $"<p><strong>Titre Original:</strong>  {dvd.TitreOriginal}</p>" +
                             $"<p><strong>Année de sortie:</strong>  {dvd.AnneeSortie}</p>" +
                             $"<p><strong>Résumé:</strong>  {dvd.ResumeFilm}</p>";

            // Combine the existing HTML message with the DVD details
            htmlMessage = $"{htmlMessage}<br/>{dvdDetails}";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("", emailSender));
            message.To.Add(new MailboxAddress("", emailReceiver));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlMessage };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpUser, _smtpPass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
        public async Task SendEmailAsync(string emailSender, List<string> emailReceiver, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("", emailSender));
            foreach (var receiver in emailReceiver)
            {
                message.To.Add(new MailboxAddress("", receiver));
            }
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlMessage };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpUser, _smtpPass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
