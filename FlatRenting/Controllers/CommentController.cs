using FlatRenting.Data;
using FlatRenting.DTOs;
using FlatRenting.Entities;
using FlatRenting.Exceptions;
using FlatRenting.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlatRenting.Controllers;
[Route("api/[controller]")]
public class CommentController : RestrictedApiController {
    private readonly ICommentRepository _commentRepository;
    private readonly IAnnoucementRepository _annoucementRepository;
    private readonly ILogger _logger;

    public CommentController(ILogger logger, ICommentRepository commentRepository, IAnnoucementRepository annoucementRepository) {
        _logger = logger;
        _commentRepository = commentRepository;
        _annoucementRepository = annoucementRepository;
    }

    [HttpGet("{annoucementId}")]
    public async Task<IActionResult> GetComments(Guid annoucementId) {
        try {
            var comments = (await _commentRepository.GetComments(annoucementId)).Select(c => c.ToDto());
            return Ok(comments);
        } catch (RepositoryException ex) {
            _logger.Error(ex, $"Cannot fetch comments for annoucement with Id '{annoucementId}'");
            return BadRequest("Cannot fetch comments for given annoucement");
        }
    }

    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteComment(Guid commentId) {
        try {
            var comment = await _commentRepository.GetComment(commentId);

            if (comment.Owner.Id != GetAuthorizedUserFromCtx().Id) {
                return BadRequest("Cannot delete unowned comment");
            }

            await _commentRepository.DeleteComment(comment);

            return Ok();
        } catch (RepositoryException ex) {
            _logger.Error(ex, $"Cannot delete comment with Id '{commentId}'");
            return BadRequest("Cannot fetch comments for given annoucement");
        }
    }

    [HttpPost("{annoucementId}")]
    public async Task<IActionResult> AddComment(Guid annoucementId, CreateCommentDto commentDto) {
        try {
            var user = GetAuthorizedUserFromCtx();
            var annoucement = await _annoucementRepository.GetAnnoucement(annoucementId);
            await _commentRepository.AddComment(commentDto, annoucement, user);
            return Ok();
        } catch (RepositoryException ex) {
            _logger.Error(ex, $"Cannot add comment to annoucement with Id '{annoucementId}'");
            return BadRequest("Cannot add comment to annoucement");
        }
    }
}
