using FlatRenting.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlatRenting.Data;

public class FlatRentingContext : DbContext {
    private readonly IConfiguration _config;
    public DbSet<User> Users { get; set; }
    public DbSet<Annoucement> Annoucements { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Activation> Activations { get; set; }

    public FlatRentingContext(DbContextOptions<FlatRentingContext> options, IConfiguration config) : base(options) {
        _config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_config.GetConnectionString("DefaultConnection"));

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Owner)
            .WithMany(o => o.Comments);

        modelBuilder.Entity<Annoucement>()
            .HasOne(a => a.Owner)
            .WithMany(o => o.Annoucements);
    }
}
