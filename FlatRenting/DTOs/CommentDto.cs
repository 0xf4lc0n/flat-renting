namespace FlatRenting.DTOs;

public class CreateCommentDto
{
    public required string Content { get; set; }
}

public class GetCommentDto
{
    public required Guid Id { get; init; }
    public required string Content { get; set; }
    public required string UserName { get; set; }
    public required Guid OwnerId { get; init; }
}
