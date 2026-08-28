using MarketPlace.Shared.Domain.Primitives;

namespace MarketPlace.Shared.Domain.UnitTests;

public class EntityTests
{
    private sealed class TestEntity : Entity
    {
        public TestEntity(Guid id) : base(id) { }
    }

    private sealed class AnotherEntity : Entity
    {
        public AnotherEntity(Guid id) : base(id) { }
    }

    [Fact]
    public void Entities_WithSameIdAndType_ShouldBeEqual()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        Assert.True(entity1 == entity2);
        Assert.True(entity1.Equals(entity2));
        Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
    }

    [Fact]
    public void Entities_WithDifferentIds_ShouldNotBeEqual()
    {
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        Assert.True(entity1 != entity2);
        Assert.False(entity1.Equals(entity2));
    }

    [Fact]
    public void Entities_WithSameIdButDifferentTypes_ShouldNotBeEqual()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new AnotherEntity(id);

        Assert.False(entity1.Equals(entity2));
    }

    [Fact]
    public void Entity_WithEmptyId_ShouldNotBeEqualToAnotherEntityWithEmptyId()
    {
        var entity1 = new TestEntity(Guid.Empty);
        var entity2 = new TestEntity(Guid.Empty);

        Assert.False(entity1.Equals(entity2));
        Assert.True(entity1 != entity2);
    }
}
