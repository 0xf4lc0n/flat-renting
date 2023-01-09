namespace FlatRenting.DTOs;

public class UserDto
{
    public required Guid Id { get; init; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Phone { get; set; }
    public string? Bio { get; set; }
    public required DateTime RegistrationDate { get; init; }
}

public class EditUserDto
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? Password { get; set; }
    public required string Phone { get; set; }
    public string? Bio { get; set; }
}

public class LoggedUsedDto {
    public required Guid Id { get; init; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Login { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public string? Bio { get; set; }
}

public class AnnoucementOwnerDto {
    public required Guid Id { get; init; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public string? Bio { get; set; }
    public required DateTime RegistrationDate { get; init; }
}