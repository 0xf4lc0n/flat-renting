namespace FlatRenting.Interfaces;

public interface ICryptoService {
    string HashPassword(string plainPassword);
    bool IsPasswordValid(string plainPassword, string hashedPassword);
    RegisterDto HashPassword(RegisterDto registerDto);
    LoginDto HashPassword(LoginDto loginDto); 
}
