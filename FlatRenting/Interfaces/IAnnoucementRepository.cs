using FlatRenting.DTOs;
using FlatRenting.Entities;
using System.ComponentModel;

namespace FlatRenting.Interfaces;

public interface IAnnoucementRepository {
    Task AddAnnoucement(AnnoucementDto annoucement, User ower);
    Task<IEnumerable<Annoucement>> GetAllAnnoucements();
    Task<Annoucement> GetAnnoucement(Guid id);
    Task<IEnumerable<Annoucement>> GetAnnoucements(Guid ownerId);
    Task UpdateAnnoucement(Guid id, Annoucement newAnnoucement);
    Task DeleteAnnoucement(Guid id);
}
