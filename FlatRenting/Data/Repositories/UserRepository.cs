using FlatRenting.DTOs;
using FlatRenting.Entities;
using FlatRenting.Exceptions;
using FlatRenting.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlatRenting.Data.Repositories;

public class UserRepository : IUserRepository {
    private readonly FlatRentingContext _ctx;
    public UserRepository(FlatRentingContext ctx) => _ctx = ctx;
    public async Task<Guid> AddUser(RegisterDto user) {
        if (await _ctx.Users.AnyAsync(u => u.Email.Equals(user.Email) || u.Login.Equals(user.Login))) {
            throw new RepositoryException("User with given email or login already exists");
        }

        var entity = user.ToEntity();
        await _ctx.Users.AddAsync(entity);
        await _ctx.SaveChangesAsync();

        return entity.Id;
    }
    public async Task<User> GetUser(Guid id) {
        try {
            return await _ctx.Users.FirstAsync(u => u.Id == id);
        } catch (Exception ex) {
            throw new RepositoryException($"Cannot get user with Id '{id}'", ex);
        }
    }

    public async Task<User> GetUser(string login, string password) {
        try {
            return await _ctx.Users.FirstAsync(u => u.Login == login && u.Password == password);
        } catch (Exception ex) {
            throw new RepositoryException($"Cannot get user with login '{login}'", ex);
        }
    }

    public async Task UpdateUser(Guid id, EditUserDto newUser, User loggedUser) {
        var user = await GetUser(id);

        if (await _ctx.Users.AnyAsync(u => u.Email.Equals(newUser.Email) && !u.Login.Equals(user.Login))) {
            throw new RepositoryException("User with given email or login already exists");
        }

        user.Email = newUser.Email;
        user.FirstName = newUser.FirstName;
        user.LastName = newUser.LastName;
        user.Phone = newUser.Phone;
        user.Bio = newUser.Bio;

        if (!string.IsNullOrWhiteSpace(newUser.Password)) {
            user.Password = newUser.Password;
        }

        _ctx.Users.Update(user);
        await _ctx.SaveChangesAsync();
    }

    public async Task ActivateUser(Guid userId) {
        var user = await GetUser(userId);
        user.IsActive = true;
        _ctx.Users.Update(user);
        await _ctx.SaveChangesAsync();
    }

    public async Task DeleteUser(Guid userId) {
        var user = await GetUser(userId);
        _ctx.Users.Remove(user);
        await _ctx.SaveChangesAsync();
    }
}
