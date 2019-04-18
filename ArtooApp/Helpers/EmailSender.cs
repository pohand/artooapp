using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Artoo.Helpers
{
    public class EmailSender
    {
        public static IConfiguration Configuration { get; set; }
        public static string emailAddress;
        public static string password;
        public EmailSender()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            emailAddress = Configuration.GetSection("EmailConfiguration:EmailAddress").Value;
            password = Configuration.GetSection("EmailConfiguration:Password").Value;
        }
        public async Task SendEmailAsync(List<string> emails, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Artoo", emailAddress));
            foreach (var item in emails)
            {
                emailMessage.Cc.Add(new MailboxAddress("", item));
            }

            emailMessage.Subject = subject;
            //emailMessage.Body = new TextPart("plain") { Text = message };
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message;
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                //client.LocalDomain = "some.domain.com";
                //await client.ConnectAsync("smtp.gmail.com", 25, SecureSocketOptions.None).ConfigureAwait(false);
                //await client.SendAsync(emailMessage).ConfigureAwait(false);
                //await client.DisconnectAsync(true).ConfigureAwait(false);
                await client.ConnectAsync("smtp.gmail.com", 587);


                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(emailAddress, password);

                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

    }
}
