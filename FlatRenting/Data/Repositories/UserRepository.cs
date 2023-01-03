using FlatRenting.DTOs;
using FlatRenting.Entities;
using FlatRenting.Exceptions;
using FlatRenting.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlatRenting.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly FlatRentingContext _ctx;
    public UserRepository(FlatRentingContext ctx) => _ctx = ctx;
    public async Task AddUser(RegisterDto user)
    {
        if (await _ctx.Users.AnyAsync(u => u.Email.Equals(user.Email) || u.Login.Equals(user.Login)))
        {
            throw new RepositoryException("User with given email or login already exists");
        }

        var entity = user.ToEntity();
        await _ctx.Users.AddAsync(entity);
        await _ctx.SaveChangesAsync();
    }
    public async Task<User> GetUser(Guid id)
    {
        try
        {
            return await _ctx.Users.FirstAsync(u => u.Id == id);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Cannot get user with Id '{id}'", ex);
        }
    }

    public async Task<User> GetUser(string login, string password) {
        try
        {
            return await _ctx.Users.FirstAsync(u => u.Login == login && u.Password == password);
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Cannot get user with login '{login}'", ex);
        }
    }

    public async Task UpdateUser(Guid id, EditUserDto newUser, User loggedUser)
    {
        var user = await GetUser(id);

        if (await _ctx.Users.AnyAsync(u => u.Email.Equals(newUser.Email) && !u.Login.Equals(user.Login)))
        {
            throw new RepositoryException("User with given email or login already exists");
        }

        user.Email = newUser.Email;
        user.FirstName = newUser.FirstName;
        user.LastName = newUser.LastName;
        user.Phone = newUser.Phone;
        user.Password = newUser.Password;
        user.Bio = newUser.Bio;

        _ctx.Users.Update(user);
        await _ctx.SaveChangesAsync();
    }
}
