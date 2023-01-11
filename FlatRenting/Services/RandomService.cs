using FlatRenting.Interfaces;

namespace FlatRenting.Services;

public class RandomService : IRandomService {
    private static readonly string _chars = "abcdefghijklmnoprstuvwxyzABCDEFGHIJKLMNOPRSTUVWXYZ0123456789";
    private readonly Random _random = new();
    public string GenerateRandomString(int len) => new(Enumerable.Repeat(_chars, len).Select(s => s[_random.Next(s.Length)]).ToArray());
}
