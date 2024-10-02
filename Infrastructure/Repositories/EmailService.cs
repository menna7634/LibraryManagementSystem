using Application.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress("Library MemberShip", "mennamohammed178@gmail.com")); // sender's email
        mailMessage.To.Add(new MailboxAddress("", email)); // Recipient's email
        mailMessage.Subject = subject;
        mailMessage.Body = new TextPart("html") { Text = message };

        using (var client = new MailKit.Net.Smtp.SmtpClient())
        {
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

            await client.AuthenticateAsync("mennamohammed178@gmail.com", "zsul dqrt raeb kdfg");

            await client.SendAsync(mailMessage);

            await client.DisconnectAsync(true);
        }
    }
}
