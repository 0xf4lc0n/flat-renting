using FlatRenting.DTOs;
using FlatRenting.Entities;
using FlatRenting.Exceptions;
using FlatRenting.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlatRenting.Data.Repositories;

public class AnnoucementRepository : IAnnoucementRepository {
    private readonly FlatRentingContext _ctx;
    public AnnoucementRepository(FlatRentingContext ctx) => _ctx = ctx;
    public async Task AddAnnoucement(CreateAnnoucementDto annoucement, User ower) {
        var entity = annoucement.ToEntity(ower);
        await _ctx.Annoucements.AddAsync(entity);
        await _ctx.SaveChangesAsync();
    }

    public async Task DeleteAnnoucement(Guid id) {
        var annoucement = await GetAnnoucement(id);
        _ctx.Annoucements.Remove(annoucement);
        await _ctx.SaveChangesAsync();
    }

    public async Task<IEnumerable<Annoucement>> GetAllAnnoucements() {
        return await _ctx.Annoucements.Include(a => a.Owner).Include(a => a.Comments).ThenInclude(c => c.Owner).ToListAsync();
    }

    public async Task<Annoucement> GetAnnoucement(Guid id) {
        try {
            return await _ctx.Annoucements.Include(a => a.Owner).Include(a => a.Comments).ThenInclude(c => c.Owner).FirstAsync(a => a.Id == id);
        } catch (Exception ex) {
            throw new RepositoryException($"Cannot get annoucement with Id '{id}'", ex);
        }

    }

    public async Task<IEnumerable<Annoucement>> GetAnnoucements(Guid ownerId) {
        try {
            return await _ctx.Annoucements.Where(a => a.OwnerId == ownerId).Include(a => a.Owner).Include(a => a.Comments).ThenInclude(c => c.Owner).ToListAsync();
        } catch (Exception ex) {
            throw new RepositoryException($"Cannot get annoucements of user with Id '{ownerId}'", ex);
        }
    }

    public async Task UpdateAnnoucement(Guid id, CreateAnnoucementDto newAnnoucement) {
        var annoucement = await GetAnnoucement(id);
        
        annoucement.Area = newAnnoucement.Area;
        annoucement.Address = newAnnoucement.Address;
        annoucement.Price = newAnnoucement.Price;
        annoucement.Pictures = newAnnoucement.Pictures;
        annoucement.Description = newAnnoucement.Description;
        annoucement.Title= newAnnoucement.Title;
        annoucement.FloorsNumber = newAnnoucement.FloorsNumber;
        annoucement.YearBuild = newAnnoucement.YearBuild;
        annoucement.RoomsNumber = newAnnoucement.RoomsNumber;

        _ctx.Annoucements.Update(annoucement);
        await _ctx.SaveChangesAsync();

    }
}
