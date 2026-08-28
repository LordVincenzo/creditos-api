using Creditos.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Creditos.Api.Data.Migrations;

[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "8.0.30");

        modelBuilder.Entity("Creditos.Api.Entities.User", b =>
        {
            b.Property<Guid>("Id").HasColumnType("uuid");
            b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("timestamp with time zone");
            b.Property<string>("DisplayName").IsRequired().HasMaxLength(120).HasColumnType("character varying(120)");
            b.Property<string>("Email").IsRequired().HasMaxLength(320).HasColumnType("character varying(320)");
            b.Property<bool>("IsActive").HasColumnType("boolean");
            b.Property<string>("PasswordHash").IsRequired().HasMaxLength(500).HasColumnType("character varying(500)");
            b.HasKey("Id");
            b.HasIndex("Email").IsUnique();
            b.ToTable("users");
        });

        modelBuilder.Entity("Creditos.Api.Entities.Credit", b =>
        {
            b.Property<Guid>("Id").HasColumnType("uuid");
            b.Property<decimal>("Amount").HasPrecision(18, 2).HasColumnType("numeric(18,2)");
            b.Property<string>("ClientDocument").IsRequired().HasMaxLength(50).HasColumnType("character varying(50)");
            b.Property<string>("ClientName").IsRequired().HasMaxLength(150).HasColumnType("character varying(150)");
            b.Property<string>("CommercialNameSnapshot").IsRequired().HasMaxLength(120).HasColumnType("character varying(120)");
            b.Property<DateTimeOffset>("CreatedAtUtc").HasColumnType("timestamp with time zone");
            b.Property<decimal>("InterestRate").HasPrecision(5, 2).HasColumnType("numeric(5,2)");
            b.Property<Guid>("RegisteredByUserId").HasColumnType("uuid");
            b.Property<int>("TermMonths").HasColumnType("integer");
            b.HasKey("Id");
            b.HasIndex("Amount");
            b.HasIndex("ClientDocument");
            b.HasIndex("ClientName");
            b.HasIndex("CreatedAtUtc");
            b.HasIndex("RegisteredByUserId");
            b.ToTable("credits");
        });

        modelBuilder.Entity("Creditos.Api.Entities.Credit", b =>
        {
            b.HasOne("Creditos.Api.Entities.User", "RegisteredByUser")
                .WithMany("Credits")
                .HasForeignKey("RegisteredByUserId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            b.Navigation("RegisteredByUser");
        });

        modelBuilder.Entity("Creditos.Api.Entities.User", b => b.Navigation("Credits"));
    }
}
