using MarketPlace.Shared.Domain.Results;

namespace MarketPlace.Shared.Domain.UnitTests;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void Failure_ShouldCreateFailedResultWithError()
    {
        // Arrange
        var error = Error.Failure("Test.Error", "A test error occurred.");

        // Act
        var result = Result.Failure(error);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void GenericSuccess_ShouldContainValueAndBeSuccess()
    {
        // Arrange
        var expectedData = "MarketPlace Payload";

        // Act
        var result = Result.Success(expectedData);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(expectedData, result.Value);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void GenericFailure_AccessingValue_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var error = Error.NotFound("User.NotFound", "User not found.");
        var result = Result.Failure<string>(error);

        // Act & Assert
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
        Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    }

    [Fact]
    public void Create_WithNonNullValue_ShouldReturnSuccess()
    {
        // Act
        var result = Result.Create("ValidValue");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("ValidValue", result.Value);
    }

    [Fact]
    public void Create_WithNullValue_ShouldReturnFailureWithNullValueError()
    {
        // Act
        var result = Result.Create<string>(null);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(Error.NullValue, result.Error);
    }
}
