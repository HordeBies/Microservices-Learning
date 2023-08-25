using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Ordering.Application.Models;
using Ordering.Application.ServiceContracts;
using Org.BouncyCastle.Security.Certificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings emailSettings;
        private readonly ILogger<EmailService> logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            this.emailSettings = emailSettings.Value ?? throw new ArgumentNullException(nameof(EmailSettings));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> SendEmail(Email email)
        {
            var emailToSend = new MimeMessage()
            {
                From = { new MailboxAddress(emailSettings.FromName, emailSettings.FromAddress) },
                To = { MailboxAddress.Parse(email.To) },
                Subject = email.Subject,
            };

            var body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = email.Body };
            var multiPart = new Multipart("mixed") { body };

            foreach (var attachment in email.Attachments)
            {
                var att = new MimePart(attachment.MediaType, attachment.MediaSubType)
                {
                    Content = new MimeContent(attachment.Content),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = attachment.FileName
                };
                multiPart.Add(att);
            }

            emailToSend.Body = multiPart;

            try
            {
                using var client = new SmtpClient();
                client.Connect(emailSettings.SmtpServer, emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate(emailSettings.FromAddress, emailSettings.Password);
                await client.SendAsync(emailToSend);
                client.Disconnect(true);
            }
            catch (Exception)
            {
                logger.LogError($"Failed to send Email to {email.To}.");
                return false;
            }

            logger.LogInformation($"Email sent to {email.To}.");
            return true;
        }
    }
}
