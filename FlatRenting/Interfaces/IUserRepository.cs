using FlatRenting.DTOs;
using FlatRenting.Entities;

namespace FlatRenting.Interfaces;

public interface IUserRepository {
    Task AddUser(UserDto user);
    Task<User> GetUser(Guid id);
    Task UpdateUser(Guid id, UserDto newUser);
}
