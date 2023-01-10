using FlatRenting.DTOs;
using FlatRenting.Interfaces;
using FlatRenting.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace FlatRenting.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ContactController : ControllerBase {
    private readonly IEmailService _email;

    public ContactController(IEmailService email) => _email = email;

    [HttpPost]
    public IActionResult SendEmail(TicketDto ticket) {
        var receiver = new ReceiverData("flat.renatl@gmail.com", "Flat Lender");

        var content = "Nazwa użytkownika: " + ticket.UserName + "\n" +
                      "Numer telefonu: " + ticket.PhoneNumber + "\n" +
                      "Adres email: " + ticket.Email + "\n" +
                      "Opis problemu: " + ticket.ProblemDescription + "\n";

        var message = new EmailData("Zgłoszenie Problemu - " + ticket.UserName, content, content);

        _email.SendEmail(receiver, message);

        return Ok();
    }
}
