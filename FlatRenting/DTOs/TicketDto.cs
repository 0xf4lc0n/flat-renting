namespace FlatRenting.DTOs;

public class TicketDto {
    public required string UserName { get; init; }
    public required string Email { get; init; }
    public required string PhoneNumber { get; init; }
    public required string ProblemDescription { get; init; }
}
