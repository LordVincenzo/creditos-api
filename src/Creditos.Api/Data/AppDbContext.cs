using Creditos.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Creditos.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Credit> Credits => Set<Credit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Email).HasMaxLength(320).IsRequired();
            entity.Property(user => user.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(user => user.DisplayName).HasMaxLength(120).IsRequired();
            entity.Property(user => user.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasIndex(user => user.Email).IsUnique();
        });

        modelBuilder.Entity<Credit>(entity =>
        {
            entity.ToTable("credits");
            entity.HasKey(credit => credit.Id);
            entity.Property(credit => credit.ClientName).HasMaxLength(150).IsRequired();
            entity.Property(credit => credit.ClientDocument).HasMaxLength(50).IsRequired();
            entity.Property(credit => credit.Amount).HasPrecision(18, 2).IsRequired();
            entity.Property(credit => credit.InterestRate).HasPrecision(5, 2).IsRequired();
            entity.Property(credit => credit.CommercialNameSnapshot).HasMaxLength(120).IsRequired();
            entity.Property(credit => credit.CreatedAtUtc).HasColumnType("timestamp with time zone");
            entity.HasOne(credit => credit.RegisteredByUser)
                .WithMany(user => user.Credits)
                .HasForeignKey(credit => credit.RegisteredByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(credit => credit.ClientName);
            entity.HasIndex(credit => credit.ClientDocument);
            entity.HasIndex(credit => credit.RegisteredByUserId);
            entity.HasIndex(credit => credit.CreatedAtUtc);
            entity.HasIndex(credit => credit.Amount);
        });
    }
}
