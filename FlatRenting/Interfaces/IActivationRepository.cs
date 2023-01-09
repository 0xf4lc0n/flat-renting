using FlatRenting.Entities;

namespace FlatRenting.Interfaces;

public interface IActivationRepository {
    Task<Guid> AddActivation(Guid userId);
    Task<Activation> GetActivation(Guid userId);
    Task DeleteActivation(Activation activation);
}
