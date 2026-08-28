using MarketPlace.Shared.Domain.Primitives;

namespace MarketPlace.Shared.Domain.UnitTests;

public class AggregateRootTests
{
    private sealed record TestDomainEvent(string Name) : IDomainEvent;

    private sealed class OrderAggregate : AggregateRoot
    {
        public OrderAggregate(Guid id) : base(id) { }

        public void PlaceOrder()
        {
            RaiseDomainEvent(new TestDomainEvent("OrderPlaced"));
        }
    }

    [Fact]
    public void RaiseDomainEvent_ShouldAddEventToDomainEventsCollection()
    {
        var order = new OrderAggregate(Guid.NewGuid());

        order.PlaceOrder();

        Assert.Single(order.DomainEvents);
        var domainEvent = Assert.IsType<TestDomainEvent>(order.DomainEvents.First());
        Assert.Equal("OrderPlaced", domainEvent.Name);
    }

    [Fact]
    public void ClearDomainEvents_ShouldEmptyTheDomainEventsCollection()
    {
        var order = new OrderAggregate(Guid.NewGuid());
        order.PlaceOrder();
        Assert.NotEmpty(order.DomainEvents);

        order.ClearDomainEvents();

        Assert.Empty(order.DomainEvents);
    }
}
