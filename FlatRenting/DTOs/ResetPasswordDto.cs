namespace FlatRenting.DTOs;

public record ResetPasswordDto(string Email, string Code, string NewPassword);

