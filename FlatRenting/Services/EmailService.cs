using FlatRenting.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using SendGrid.Helpers.Mail.Model;
using System;
using System.Threading.Tasks;

namespace FlatRenting.Services;

public class EmailService : IEmailService {
    private readonly IConfiguration _config;
    private readonly ILogger _logger;

    public EmailService(IConfiguration config, ILogger logger) {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmail(ReceiverData receiver, EmailData email) {
        var apiKey = _config["Email:ApiKey"];

        try {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("flat.renatl@gmail.com", "Flat Lender");
            var to = new EmailAddress(receiver.Email, receiver.FullName);
            var msg = MailHelper.CreateSingleEmail(from, to, email.Subject, email.PlainTextContent, email.HtmlContent);
            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode) {
                _logger.Debug("Response: {@Response}", response);
                throw new EmailException("SendGrid returned non OK code");
            }
        } catch (Exception ex) {
            throw new EmailException("Cannot send email message", ex);
        }
    }
}

public record ReceiverData(string Email, string FullName);
public record EmailData(string Subject, string PlainTextContent, string HtmlContent);
