using FlatRenting.Data;
using FlatRenting.DTOs;
using FlatRenting.Entities;
using FlatRenting.Exceptions;
using FlatRenting.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlatRenting.Controllers;
[Route("api/[controller]")]
public class AnnoucementController : RestrictedApiController {
    private readonly IAnnoucementRepository _annoucementRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger _logger;

    public AnnoucementController(IAnnoucementRepository annoucementRepository, IUserRepository userRepository, ILogger logger) {
        _annoucementRepository = annoucementRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAnnoucement(CreateAnnoucementDto createAnnoucementDto) {
        try {
            await _annoucementRepository.AddAnnoucement(createAnnoucementDto, GetAuthorizedUserFromCtx());
        } catch (RepositoryException ex) {
            _logger.Error(ex, "Cannot add annoucement @{Annoucement}", createAnnoucementDto);
            return BadRequest("Cannot add new annoucement");
        }

        return Ok();
    }

    [HttpDelete("{annoucementId}")]
    public async Task<IActionResult> DeleteAnnoucement(Guid annoucementId) {
        var annoucement = await _annoucementRepository.GetAnnoucement(annoucementId);

        if (annoucement.OwnerId != GetAuthorizedUserFromCtx().Id) {
            return BadRequest("Cannot delete unowned annoucement");
        }

        await _annoucementRepository.DeleteAnnoucement(annoucementId);
        return Ok();
    }

    [HttpPatch("{annoucementId}")]
    public async Task<IActionResult> UpdateAnnoucement(Guid annoucementId, CreateAnnoucementDto annoucementDto) {
        var annoucement = await _annoucementRepository.GetAnnoucement(annoucementId);

        if (annoucement.OwnerId != GetAuthorizedUserFromCtx().Id) {
            return BadRequest("Cannot update unowned annoucement");
        }

        await _annoucementRepository.UpdateAnnoucement(annoucementId, annoucementDto);
        return Ok();
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllAnnoucements() {
        var annoucements = (await _annoucementRepository.GetAllAnnoucements()).Select(a => a.ToDto());
        return Ok(annoucements);
    }

    [HttpGet("{userId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllAnnoucements(Guid userId) {
        var annoucements = (await _annoucementRepository.GetAnnoucements(userId)).Select(a => a.ToDto());;
        return Ok(annoucements);
    }


    [HttpGet("details/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAnnoucement(Guid annoucementId) {
        var annoucements = (await _annoucementRepository.GetAnnoucement(annoucementId)).ToDto();
        return Ok(annoucements);
    }
}
