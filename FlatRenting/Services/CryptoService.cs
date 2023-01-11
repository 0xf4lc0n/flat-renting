namespace FlatRenting.Services;

using BCrypt.Net;
using FlatRenting.Interfaces;

public class CryptoService : ICryptoService {
    // Returns hash encoded in Base64. Hash contains salt and hahsed password which are splited via ':' character.
    public string HashPassword(string plainPassword) {
        var salt = BCrypt.GenerateSalt();
        var hash = BCrypt.HashPassword(plainPassword, salt);
        return hash;
    }

    public RegisterDto HashPassword(RegisterDto registerDto)
        => new RegisterDto {
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Login = registerDto.Login,
            Password = HashPassword(registerDto.Password),
            Phone = registerDto.Phone
        };
    public LoginDto HashPassword(LoginDto loginDto) => new LoginDto {
        Login = loginDto.Login,
        Password = HashPassword(loginDto.Password)
    };

    public bool IsPasswordValid(string plainPassword, string hashedPassword) => BCrypt.Verify(plainPassword, hashedPassword);
}
