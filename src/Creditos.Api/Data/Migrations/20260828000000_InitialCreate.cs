using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Creditos.Api.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260828000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                DisplayName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_users", x => x.Id));

        migrationBuilder.CreateTable(
            name: "credits",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ClientName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                ClientDocument = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                InterestRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                TermMonths = table.Column<int>(type: "integer", nullable: false),
                RegisteredByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                CommercialNameSnapshot = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_credits", x => x.Id);
                table.ForeignKey("FK_credits_users_RegisteredByUserId", x => x.RegisteredByUserId, "users", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(name: "IX_users_Email", table: "users", column: "Email", unique: true);
        migrationBuilder.CreateIndex(name: "IX_credits_ClientName", table: "credits", column: "ClientName");
        migrationBuilder.CreateIndex(name: "IX_credits_ClientDocument", table: "credits", column: "ClientDocument");
        migrationBuilder.CreateIndex(name: "IX_credits_RegisteredByUserId", table: "credits", column: "RegisteredByUserId");
        migrationBuilder.CreateIndex(name: "IX_credits_CreatedAtUtc", table: "credits", column: "CreatedAtUtc");
        migrationBuilder.CreateIndex(name: "IX_credits_Amount", table: "credits", column: "Amount");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "credits");
        migrationBuilder.DropTable(name: "users");
    }
}
