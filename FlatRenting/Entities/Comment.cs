namespace FlatRenting.Entities;

public class Comment {
    public required Guid Id { get; init; }
    public required string Content { get; set; }
    public required Guid AnnoucementId { get; init; }
    public required Guid OwnerId { get; init; }
    public required User Owner { get; init; }
}
