using FlatRenting.DTOs;
using FlatRenting.Entities;

namespace FlatRenting.Interfaces;

public interface IUserRepository {
    Task<Guid> AddUser(RegisterDto user);
    Task<User> GetUser(Guid id);
    Task<User> GetUser(string login, string password);
    Task UpdateUser(Guid id, EditUserDto newUser,  User loggedUser);
    Task ActivateUser(Guid userId);
    Task DeleteUser(Guid userId);
}
