using FlatRenting.Services;

namespace FlatRenting.Interfaces;

public interface IEmailService {
    Task SendEmail(ReceiverData receiver, EmailData email);
}
