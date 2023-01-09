using FlatRenting.Entities;
using FlatRenting.Exceptions;
using FlatRenting.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlatRenting.Data.Repositories;

public class ActivationRepository : IActivationRepository {
    private readonly FlatRentingContext _ctx;
    public ActivationRepository(FlatRentingContext ctx) => _ctx = ctx;

    public async Task<Guid> AddActivation(Guid userId) {
        var activation = new Activation { Id = Guid.NewGuid(), UserId = userId, ValidTo = DateTime.UtcNow.AddMinutes(10)};
        await _ctx.Activations.AddAsync(activation);
        await _ctx.SaveChangesAsync();
        return activation.Id;
    }

    public async Task<Activation> GetActivation(Guid userId) {
        try {
            return await _ctx.Activations.FirstAsync(a => a.UserId == userId);
        } catch(Exception ex) {
            throw new RepositoryException($"Cannot get activation for user with id '{userId}'", ex);
        }
    }

    public async Task DeleteActivation(Activation activation) {
        _ctx.Activations.Remove(activation);
        await _ctx.SaveChangesAsync();
    }
}
