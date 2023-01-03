using FlatRenting.DTOs;
using FlatRenting.Entities;

namespace FlatRenting.Interfaces;

public interface IUserRepository {
    Task AddUser(RegisterDto user);
    Task<User> GetUser(Guid id);
    Task<User> GetUser(string login, string password);
    Task UpdateUser(Guid id, UserDto newUser);
}
