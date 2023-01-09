namespace FlatRenting.Entities;

public class User {
    public required Guid Id { get; init; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Login { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Phone { get; set; }
    public string? Bio { get; set; }
    public required DateTime RegistrationDate { get; init; }
    public required bool IsActive { get; set; }
}
