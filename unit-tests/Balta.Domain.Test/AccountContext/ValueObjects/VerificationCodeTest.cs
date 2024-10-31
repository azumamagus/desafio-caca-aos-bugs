using System.Net;
using Balta.Domain.AccountContext.ValueObjects;
using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.SharedContext.DateTimes.Abstractions;
using Moq;

namespace Balta.Domain.Test.AccountContext.ValueObjects;

public class VerificationCodeTest
{
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly DateTime _dateNow;

    public VerificationCodeTest()
    {
        _dateNow = new DateTime(2024, 10, 29, 22, 0, 0, DateTimeKind.Utc);
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _dateTimeProviderMock.Setup(x => x.UtcNow).Returns(_dateNow);
    }

    [Fact]
    public void ShouldGenerateVerificationCode()
    {
        // Act
        var result = VerificationCode.ShouldCreate(_dateTimeProviderMock.Object);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Code);
        Assert.Equal(6, result.Code.Length);
        Assert.True(result.IsActive);
        Assert.Null(result.VerifiedAtUtc);
        Assert.NotNull(result.ExpiresAtUtc);
        Assert.False(result.IsExpired);
    }

    [Fact]
    public void ShouldGenerateExpiresAtInFuture()
    {
        // Arrange
        var currentTime = _dateNow;
        var expectedExpirationTime = currentTime.AddMinutes(5);

        // Act
        var result = VerificationCode.ShouldCreate(_dateTimeProviderMock.Object);

        // Assert
        Assert.Equal(expectedExpirationTime, result.ExpiresAtUtc);
    }

    [Fact]
    public void ShouldGenerateVerifiedAtAsNull()
    {
        // Act
        var result = VerificationCode.ShouldCreate(_dateTimeProviderMock.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.VerifiedAtUtc);
    }

    [Fact]
    public void ShouldBeActiveWhenCreated()
    {
        // Act
        var result = VerificationCode.ShouldCreate(_dateTimeProviderMock.Object);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsActive);
    }

    [Fact]
    public void ShouldFailIfExpired()
    {
        // Arrange
        var result = VerificationCode.ShouldCreate(_dateTimeProviderMock.Object);
        var code = result.Code;

        _dateTimeProviderMock
            .Setup(x => x.UtcNow)
            .Returns(_dateNow.AddMinutes(6));

        // Act & Assert
        Assert.Throws<InvalidVerificationCodeException>(() => result.ShouldVerify(result.Code));
    }

    [Fact]
    public void ShouldFailIfCodeIsInvalid()
    {
        // Act
        var result = VerificationCode.ShouldCreate(_dateTimeProviderMock.Object);
        result.SetCode(string.Empty);

        // Assert
        Assert.Throws<InvalidVerificationCodeException>(() => result.ShouldVerify(result.Code));
    }

    [Fact]
    public void ShouldFailIfCodeIsLessThanSixChars()
    {
        // Act
        var result = VerificationCode.ShouldCreate(_dateTimeProviderMock.Object);
        result.SetCode("12345");

        // Assert
        Assert.Throws<InvalidVerificationCodeException>(() => result.ShouldVerify(result.Code));
    }

    [Fact]
    public void ShouldFailIfCodeIsGreaterThanSixChars()
    {
        // Act
        var result = VerificationCode.ShouldCreate(_dateTimeProviderMock.Object);
        result.SetCode("1234567");

        // Assert
        Assert.Throws<InvalidVerificationCodeException>(() => result.ShouldVerify(result.Code));
    }

    [Fact]
    public void ShouldFailIfIsNotActive()
    {
        // Act
        var result = VerificationCode.ShouldCreate(_dateTimeProviderMock.Object);
        var originalCode = result.Code;

        result.ShouldVerify(originalCode);

        // Act & Assert
        Assert.Throws<InvalidVerificationCodeException>(() => result.ShouldVerify(originalCode));
    }

    [Fact]
    public void ShouldFailIfIsAlreadyVerified()
    {
        // Act
        var result = VerificationCode.ShouldCreate(_dateTimeProviderMock.Object);
        result.SetVerifiedAt(DateTime.UtcNow);

        // Assert
        Assert.Throws<InvalidVerificationCodeException>(() => result.ShouldVerify(result.Code));
    }

    [Fact]
    public void ShouldFailIfIsVerificationCodeDoesNotMatch()
    {
        // Arrange
        const string wrongCode = "DEF456";
        var verificationCode = VerificationCode.ShouldCreate(_dateTimeProviderMock.Object);

        // Act & Assert
        Assert.Throws<InvalidVerificationCodeException>(() => verificationCode.ShouldVerify(wrongCode));
    }

    [Fact]
    public void ShouldVerify()
    {
        // Arrange
        var verificationCode = VerificationCode.ShouldCreate(_dateTimeProviderMock.Object);
        var code = verificationCode.Code;

        //Act
        verificationCode.ShouldVerify(code);

        // Assert
        Assert.Equal(_dateNow, verificationCode.VerifiedAtUtc);
        Assert.Null(verificationCode.ExpiresAtUtc);
    }
}