using FlatRenting.DTOs;
using FlatRenting.Entities;

namespace FlatRenting.Interfaces;

public interface IUserRepository {
    Task<Guid> AddUser(RegisterDto user);
    Task<User> GetUser(Guid id);
    Task<User> GetUserByLogin(string login);
    Task<User> GetUser(string email);
    Task UpdateUser(Guid id, EditUserDto newUser,  User loggedUser);
    Task ActivateUser(Guid userId);
    Task DeleteUser(Guid userId);
    Task ChangeUserPassword(string userEmail, string newPassword);
}
