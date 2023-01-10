using FlatRenting.Services;

namespace FlatRenting.Interfaces;

public interface IEmailService {
    void SendEmail(ReceiverData receiver, EmailData email);
}
