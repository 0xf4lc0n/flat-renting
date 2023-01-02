using FlatRenting.DTOs;
using FlatRenting.Entities;

namespace FlatRenting.Interfaces;

public interface ICommentRepository {
    Task AddComment(CreateCommentDto commentDto, Annoucement annoucement, User owner);
    Task<IEnumerable<Comment>> GetComment(Guid annoucementId);
}
