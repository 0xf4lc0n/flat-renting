namespace FlatRenting.Entities;

public class Activation {
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required DateTime ValidTo { get; init; }
}
