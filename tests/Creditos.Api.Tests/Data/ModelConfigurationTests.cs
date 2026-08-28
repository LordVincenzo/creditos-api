using Creditos.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Creditos.Api.Tests.Data;

public class ModelConfigurationTests
{
    [Fact]
    public void CreditModelUsesRequiredPrecisionAndIndexes()
    {
        using var db = TestDb.Create();
        var credit = db.Model.FindEntityType(typeof(Credit))!;

        Assert.Equal(18, credit.FindProperty(nameof(Credit.Amount))!.GetPrecision());
        Assert.Equal(2, credit.FindProperty(nameof(Credit.Amount))!.GetScale());
        Assert.Equal(5, credit.FindProperty(nameof(Credit.InterestRate))!.GetPrecision());
        Assert.Equal(2, credit.FindProperty(nameof(Credit.InterestRate))!.GetScale());

        var indexed = credit.GetIndexes()
            .SelectMany(index => index.Properties)
            .Select(property => property.Name)
            .ToHashSet();

        Assert.Contains(nameof(Credit.ClientName), indexed);
        Assert.Contains(nameof(Credit.ClientDocument), indexed);
        Assert.Contains(nameof(Credit.RegisteredByUserId), indexed);
        Assert.Contains(nameof(Credit.CreatedAtUtc), indexed);
        Assert.Contains(nameof(Credit.Amount), indexed);
    }

    [Fact]
    public void UserEmailIsUnique()
    {
        using var db = TestDb.Create();
        var user = db.Model.FindEntityType(typeof(User))!;
        Assert.Contains(user.GetIndexes(), index => index.IsUnique && index.Properties.Single().Name == nameof(User.Email));
    }
}
