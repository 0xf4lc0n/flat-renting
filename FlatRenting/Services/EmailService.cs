using FlatRenting.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using SendGrid.Helpers.Mail.Model;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace FlatRenting.Services;

public class EmailService : IEmailService {
    private readonly IConfiguration _config;
    private readonly ILogger _logger;

    public EmailService(IConfiguration config, ILogger logger) {
        _config = config;
        _logger = logger;
    }

    public void SendEmail(ReceiverData receiver, EmailData email) {
        var host = _config["Email:Host"];
        var port = int.Parse(_config["Email:Port"]);
        var apiKey = _config["Email:ApiKey"];
        var secretKey = _config["Email:Secretkey"];


        var client = new SmtpClient(host, port) {
            Credentials = new NetworkCredential(apiKey, secretKey),
            EnableSsl = true
        };


        try {
            client.Send("flat.renatl@gmail.com", receiver.Email, email.Subject, email.PlainTextContent);
        } catch (Exception ex) {
            throw new EmailException("Cannot send email message", ex);
        }
    }
}

public record ReceiverData(string Email, string FullName);
public record EmailData(string Subject, string PlainTextContent, string HtmlContent);
