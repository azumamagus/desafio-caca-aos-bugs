using Balta.Domain.AccountContext.ValueObjects;
using Balta.Domain.AccountContext.ValueObjects.Exceptions;

namespace Balta.Domain.Test.AccountContext.ValueObjects;

public class PasswordTests
{
    [Fact]
    public void ShouldFailIfPasswordIsNull()
    {
        // Arrange
        string plainText = null;

        // Act & Assert
        Assert.Throws<InvalidPasswordException>(() => Password.ShouldCreate(plainText));
    }

    [Fact]
    public void ShouldFailIfPasswordIsEmpty()
    {
        // Arrange
        string plainText = string.Empty;

        // Act & Assert
        Assert.Throws<InvalidPasswordException>(() => Password.ShouldCreate(plainText));
    }

    [Fact]
    public void ShouldFailIfPasswordIsWhiteSpace()
    {
        // Arrange
        string plainText = " ";

        // Act & Assert
        Assert.Throws<InvalidPasswordException>(() => Password.ShouldCreate(plainText));
    }

    [Fact]
    public void ShouldFailIfPasswordLenIsLessThanMinimumChars()
    {
        // Arrange
        string plainText = "1234567";

        // Act & Assert
        Assert.Throws<InvalidPasswordException>(() => Password.ShouldCreate(plainText));
    }

    [Fact]
    public void ShouldFailIfPasswordLenIsGreaterThanMaxChars()
    {
        // Arrange
        string plainText = new('a', 49);

        // Act & Assert
        Assert.Throws<InvalidPasswordException>(() => Password.ShouldCreate(plainText));
    }

    [Fact]
    public void ShouldHashPassword()
    {
        // Arrange
        string plainText = "password";

        // Act
        var result = Password.ShouldCreate(plainText);

        // Assert
        var hash = result.Hash;
        Assert.True(Password.ShouldMatch(hash, plainText));
    }

    [Fact]
    public void ShouldVerifyPasswordHash()
    {
        // Arrange
        string plainText = "password";

        // Act
        var result = Password.ShouldCreate(plainText);

        // Assert
        var hash = result.Hash;
        Assert.Equal(result.ToString(), hash);
    }

    [Fact]
    public void ShouldGenerateStrongPassword()
    {
        // Act
        var result = Password.ShouldGenerate();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(16, result.Length);
        Assert.Contains(result, x => char.IsLetterOrDigit(x) || !char.IsLetterOrDigit(x));
        Assert.Contains(result, char.IsUpper);
    }

    [Fact]
    public void ShouldImplicitConvertToString()
    {
        // Arrange
        string plainText = "password";
        var password = Password.ShouldCreate(plainText);

        // Act
        var result = (string)password;

        // Assert
        Assert.Equal(password.Hash, result);
    }

    [Fact]
    public void ShouldReturnHashAsStringWhenCallToStringMethod()
    {
        // Arrange
        string plainText = "password";
        var password = Password.ShouldCreate(plainText);

        // Act
        var result = password.ToString();

        // Assert
        Assert.Equal(password.Hash, result);
    }
    
    [Fact]
    public void ShouldMarkPasswordAsExpired()
    {
        // Arrange
        string plainText = "password";
        var password = Password.ShouldCreate(plainText);

        // Act
        var result = password.SetExpirationDate(DateTime.Now.AddDays(-1));

        // Assert
        Assert.True(result.ExpiresAtUtc.HasValue);
        Assert.True(result.ExpiresAtUtc.Value < DateTime.Now);
    }

    [Fact]
    public void ShouldFailIfPasswordIsExpired()
    {
        // Arrange
        string plainText = "password";
        var password = Password.ShouldCreate(plainText);
        var passWithExpiration = password.SetExpirationDate(DateTime.Now.AddDays(-1));

        // Act & Assert
        Assert.Throws<InvalidPasswordException>(() => passWithExpiration.ValidateExpiration(DateTime.Now));
    }

    [Fact]
    public void ShouldMarkPasswordAsMustChange()
    {
        // Arrange
        string plainText = "password";
        var password = Password.ShouldCreate(plainText);

        // Act
        var result = password.SetMustChange();

        // Assert
        Assert.True(result.MustChange);
    }

    [Fact]
    public void ShouldFailIfPasswordIsMarkedAsMustChange()
    {
        // Arrange
        string plainText = "password";
        var password = Password.ShouldCreate(plainText);
        var passWithExpiration = password.SetMustChange();

        // Act & Assert
        Assert.Throws<InvalidPasswordException>(() => passWithExpiration.ValidateMustChange());
    }
}