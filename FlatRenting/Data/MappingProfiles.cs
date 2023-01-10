using FlatRenting.DTOs;
using FlatRenting.Entities;
using System.Runtime.CompilerServices;

namespace FlatRenting.Data;

public static class MappingProfiles {
    public static UserDto ToDto(this User user) => new() {
        Id = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Password = user.Password,
        Bio = user.Bio,
        Phone = user.Phone,
        RegistrationDate = user.RegistrationDate
    };

    public static LoggedUsedDto ToLoggedDto(this User user) => new() {
        Id = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Login = user.Login,
        Bio = user.Bio,
        Phone = user.Phone,
    };

    public static AnnoucementOwnerDto ToAnnoucementOwnerDto(this User user) => new() {
        Id = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Bio = user.Bio,
        Phone = user.Phone,
        RegistrationDate = user.RegistrationDate
    };

    public static User ToEntity(this RegisterDto dto) => new() {
        Id = Guid.NewGuid(),
        Email = dto.Email,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        Login = dto.Login,
        Password = dto.Password,
        Phone= dto.Phone,
        RegistrationDate = DateTime.UtcNow,
        IsActive = false,
        Bio = string.Empty
    };

    public static Comment ToEntity(this CreateCommentDto dto, Annoucement annoucement, User owner) => new() {
        Id = Guid.NewGuid(),
        Content = dto.Content,
        AnnoucementId = annoucement.Id,
        OwnerId = owner.Id,
        Owner = owner
    };

    public static GetCommentDto ToDto(this Comment comment) => new() {
        Id =  comment.Id,
        Content = comment.Content,
        UserName = $"{comment.Owner.FirstName} {comment.Owner.LastName}"
    };

    public static Annoucement ToEntity(this CreateAnnoucementDto dto, User owner) => new() {
        Id = Guid.NewGuid(),
        Title = dto.Title,
        Address = dto.Address,
        Area = dto.Area,
        Description = dto.Description,
        FloorsNumber = dto.FloorsNumber,
        Pictures = dto.Pictures,
        Price = dto.Price,
        YearBuild = dto.YearBuild,
        RoomsNumber = dto.RoomsNumber,
        Owner = owner,
        OwnerId = owner.Id,
        Comments = new List<Comment>()
    };

    public static AnnoucementDto ToDto(this Annoucement annoucement) => new() {
        Id = annoucement.Id,
        Title = annoucement.Title,
        Address = annoucement.Address,
        Area = annoucement.Area,
        Description = annoucement.Description,
        FloorsNumber = annoucement.FloorsNumber,
        Pictures = annoucement.Pictures,
        Price = annoucement.Price,
        YearBuild = annoucement.YearBuild,
        RoomsNumber = annoucement.RoomsNumber,
        Owner = annoucement.Owner.ToAnnoucementOwnerDto(),
        Comments = annoucement.Comments.Select(c => c.ToDto()),
    };
}
