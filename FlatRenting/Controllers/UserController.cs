using FlatRenting.Data;
using FlatRenting.DTOs;
using FlatRenting.Exceptions;
using FlatRenting.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlatRenting.Controllers;
[Route("api/[controller]")]
public class UserController : RestrictedApiController {
    private readonly IUserRepository _userRepository;
    private readonly ILogger _logger;

    public UserController(IUserRepository userRepository, ILogger logger) {
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpPatch("{userId}")]
    public async Task<IActionResult> EditUser(Guid userId, EditUserDto editUserDto) {
        try {
            await _userRepository.UpdateUser(userId, editUserDto, GetAuthorizedUserFromCtx());
        } catch (RepositoryException ex) {
            _logger.Error(ex, "Cannot update user with Id {UserId}", userId);
            return BadRequest("Cannot update user");
        }

        return Ok();
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(Guid userId) {
        try {
            var user = (await _userRepository.GetUser(userId)).ToDto();
            return Ok(user);
        } catch (RepositoryException ex) {
            _logger.Error(ex, "Cannot get user with Id {UserId}", userId);
            return BadRequest("Cannot get user");
        }
    }
}
