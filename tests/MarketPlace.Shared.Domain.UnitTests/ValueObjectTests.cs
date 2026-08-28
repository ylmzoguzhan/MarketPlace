using MarketPlace.Shared.Domain.Primitives;

namespace MarketPlace.Shared.Domain.UnitTests;

public class ValueObjectTests
{
    private sealed class Money : ValueObject
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }

    [Fact]
    public void ValueObjects_WithSameProperties_ShouldBeEqual()
    {
        var money1 = new Money(100.50m, "TRY");
        var money2 = new Money(100.50m, "TRY");

        Assert.True(money1 == money2);
        Assert.True(money1.Equals(money2));
        Assert.Equal(money1.GetHashCode(), money2.GetHashCode());
    }

    [Fact]
    public void ValueObjects_WithDifferentProperties_ShouldNotBeEqual()
    {
        var money1 = new Money(100.50m, "TRY");
        var money2 = new Money(200.00m, "TRY");
        var money3 = new Money(100.50m, "USD");

        Assert.True(money1 != money2);
        Assert.False(money1.Equals(money2));
        Assert.True(money1 != money3);
        Assert.False(money1.Equals(money3));
    }
}
