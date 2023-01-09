namespace FlatRenting.DTOs;

public class AnnoucementDto
{
    public required Guid Id { get; init; }
    public required List<string> Pictures { get; set; }
    public required string Title { get; set; }
    public required decimal Price { get; set; }
    public required decimal Area { get; set; }
    public required int RoomsNumber { get; set; }
    public required int FloorsNumber { get; set; }
    public required int YearBuild { get; set; }
    public required string Address { get; set; }
    public required string Description { get; set; }
    public required AnnoucementOwnerDto Owner { get; init; }
    public required IEnumerable<GetCommentDto> Comments { get; set; }
}

public class CreateAnnoucementDto {
    public required List<string> Pictures { get; set; }
    public required string Title { get; set; }
    public required decimal Price { get; set; }
    public required decimal Area { get; set; }
    public required int RoomsNumber { get; set; }
    public required int FloorsNumber { get; set; }
    public required int YearBuild { get; set; }
    public required string Address { get; set; }
    public required string Description { get; set; }
}
