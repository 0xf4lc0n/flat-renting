using FlatRenting.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlatRenting.Data;

public class FlatRentingContext : DbContext {
    public DbSet<User> Users { get; set; }
    public DbSet<Annoucement> Annoucements { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public FlatRentingContext(DbContextOptions<FlatRentingContext> options) : base(options) {}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Database=flat-rental;Username=postgres;Password=postgres");
}
