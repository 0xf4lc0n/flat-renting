namespace FlatRenting.DTOs;

public class UserDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Login { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Phone { get; set; }
    public string? Bio { get; set; }
    public required DateTime RegistrationDate { get; init; }
}