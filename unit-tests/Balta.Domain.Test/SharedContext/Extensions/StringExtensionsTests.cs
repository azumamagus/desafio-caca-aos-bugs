using System.Text;
using Balta.Domain.SharedContext.Extensions;

namespace Balta.Domain.Test.SharedContext.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void ShouldGenerateBase64FromString()
    {
        // Arrange
        string example = "example";
        string expectedBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(example));

        // Act
        string actualBase64 = example.ToBase64();

        // Assert
        Assert.Equal(expectedBase64, actualBase64);
    }

}