using FlatRenting.DTOs;
using FlatRenting.Entities;
using FlatRenting.Exceptions;
using FlatRenting.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FlatRenting.Data.Repositories;

public class CommentRepository : ICommentRepository {
    private readonly FlatRentingContext _ctx;
    public CommentRepository(FlatRentingContext ctx) => _ctx = ctx;

    public async Task AddComment(CreateCommentDto commentDto, Annoucement annoucement, User owner) {
        var entity = commentDto.ToEntity(annoucement, owner);
        await _ctx.Comments.AddAsync(entity);
        await _ctx.SaveChangesAsync();
    }

    public async Task<IEnumerable<Comment>> GetComments(Guid annoucementId) {
        try {
            return await _ctx.Comments.Where(c => c.AnnoucementId == annoucementId).Include(c => c.Owner).ToListAsync();
        } catch (Exception ex) {
            throw new RepositoryException($"Cannot get comments for annoucement with Id '{annoucementId}'", ex);
        }
    }
}
