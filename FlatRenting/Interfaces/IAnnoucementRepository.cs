using FlatRenting.DTOs;
using FlatRenting.Entities;
using System.ComponentModel;

namespace FlatRenting.Interfaces;

public interface IAnnoucementRepository {
    Task AddAnnoucement(CreateAnnoucementDto annoucement, User ower);
    Task<IEnumerable<Annoucement>> GetAllAnnoucements();
    Task<Annoucement> GetAnnoucement(Guid id);
    Task<IEnumerable<Annoucement>> GetAnnoucements(Guid ownerId);
    Task UpdateAnnoucement(Guid id, CreateAnnoucementDto newAnnoucement);
    Task DeleteAnnoucement(Guid id);
}
