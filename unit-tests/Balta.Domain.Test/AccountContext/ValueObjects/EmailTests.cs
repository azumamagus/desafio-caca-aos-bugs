using Balta.Domain.AccountContext.ValueObjects;
using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.SharedContext.DateTimes.Abstractions;
using Balta.Domain.SharedContext.Extensions;
using Moq;

namespace Balta.Domain.Test.AccountContext.ValueObjects;

public class EmailTests
{
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

    public EmailTests()
    {
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _dateTimeProviderMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);
    }

    [Fact]
    public void ShouldLowerCaseEmail()
    {
        // Arrange
        var address = "TestE@TESte.com";

        // Act
        var result = Email.ShouldCreate(address, _dateTimeProviderMock.Object);

        // Assert
        Assert.Equal("teste@teste.com", result.Address);
    }
    
    [Fact]
    public void ShouldTrimEmail()
    {
        // Arrange
        var address = " teste@teste.com ";

        // Act
        var result = Email.ShouldCreate(address, _dateTimeProviderMock.Object);

        // Assert
        Assert.Equal("teste@teste.com", result.Address);
    }

    [Fact]
    public void ShouldFailIfEmailIsNull()
    {
        // Arrange
        string address = null;

        // Act & Assert
        Assert.Throws<NullReferenceException>(() => Email.ShouldCreate(address, _dateTimeProviderMock.Object)); 
    }

    [Fact]
    public void ShouldFailIfEmailIsEmpty()
    {
        // Arrange
        string address = string.Empty;

        // Act & Assert
        Assert.Throws<InvalidEmailException>(() => Email.ShouldCreate(address, _dateTimeProviderMock.Object));
    }
    
    [Fact]
    public void ShouldFailIfEmailIsInvalid()
    {
        // Arrange
        string address = "teste";

        // Act & Assert
        Assert.Throws<InvalidEmailException>(() => Email.ShouldCreate(address, _dateTimeProviderMock.Object));
    }

    [Fact]
    public void ShouldPassIfEmailIsValid()
    {
        // Arrange
        string address = "teste@teste.com";

        // Act
        var result = Email.ShouldCreate(address, _dateTimeProviderMock.Object);

        // Assert
        Assert.Equal("teste@teste.com", result.Address);
    }

    [Fact]
    public void ShouldHashEmailAddress()
    {
        // Arrange
        string address = "teste@teste.com";

        // Act
        var result = Email.ShouldCreate(address, _dateTimeProviderMock.Object);

        // Assert
        var addressHashed = address.ToBase64();
        Assert.Equal(result.Hash, addressHashed);
    }

    [Fact]
    public void ShouldExplicitConvertFromString()
    {
        // Arrange
        string address = "teste@teste.com";

        // Act
        string result = (Email)address;

        // Assert
        Assert.Equal(result, address);
    }

    [Fact]
    public void ShouldExplicitConvertToString()
    {
        // Arrange
        string address = "teste@teste.com";
        var email = Email.ShouldCreate(address, _dateTimeProviderMock.Object);

        // Act
        var result = (string)email;

        // Assert
        Assert.Equal(result, address);
    }

    [Fact]
    public void ShouldReturnEmailWhenCallToStringMethod()
    {
        // Arrange
        string address = "teste@teste.com";
        var email = Email.ShouldCreate(address, _dateTimeProviderMock.Object);

        // Act
        var result = email.ToString();

        // Assert
        Assert.Equal(result, address);
    }
}