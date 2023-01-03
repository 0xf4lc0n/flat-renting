using FlatRenting.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlatRenting.Data;

public class FlatRentingContext : DbContext {
    private readonly IConfiguration _config;
    public DbSet<User> Users { get; set; }
    public DbSet<Annoucement> Annoucements { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public FlatRentingContext(DbContextOptions<FlatRentingContext> options, IConfiguration config) : base(options) {
        _config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_config.GetConnectionString("DefaultConnection"));
}
