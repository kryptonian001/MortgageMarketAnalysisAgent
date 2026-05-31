using FluentAssertions;
using MortgageMarketAnalysisAgent.Helpers;
using MortgageMarketAnalysisAgent.Models.Documents.Components;

namespace MortgageMarketAnalysisAgent.Tests.Helpers;

public class GoogleSheetHelpersTests
{
    #region SafeString Tests

    [Fact]
    public void SafeString_WithValidIndex_ReturnsValue()
    {
        // Arrange
        IList<object> row = new List<object> { "First", "Second", "Third" };

        // Act
        var result = row.SafeString(1);

        // Assert
        result.Should().Be("Second");
    }

    [Fact]
    public void SafeString_WithFirstIndex_ReturnsFirstValue()
    {
        // Arrange
        IList<object> row = new List<object> { "Value1", "Value2" };

        // Act
        var result = row.SafeString(0);

        // Assert
        result.Should().Be("Value1");
    }

    [Fact]
    public void SafeString_WithLastIndex_ReturnsLastValue()
    {
        // Arrange
        IList<object> row = new List<object> { "A", "B", "C" };

        // Act
        var result = row.SafeString(2);

        // Assert
        result.Should().Be("C");
    }

    [Fact]
    public void SafeString_WithIndexOutOfRange_ReturnsEmptyString()
    {
        // Arrange
        IList<object> row = new List<object> { "Only", "Two" };

        // Act
        var result = row.SafeString(5);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void SafeString_WithNegativeIndex_ReturnsEmptyString()
    {
        // Arrange
        IList<object> row = new List<object> { "Value" };

        // Act
        var result = row.SafeString(-1);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void SafeString_WithEmptyRow_ReturnsEmptyString()
    {
        // Arrange
        IList<object> row = new List<object>();

        // Act
        var result = row.SafeString(0);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void SafeString_WithNullValue_ReturnsEmptyString()
    {
        // Arrange
        IList<object> row = new List<object> { null!, "Value" };

        // Act
        var result = row.SafeString(0);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void SafeString_WithNumericValue_ReturnsStringRepresentation()
    {
        // Arrange
        IList<object> row = new List<object> { 123, 45.67 };

        // Act
        var result1 = row.SafeString(0);
        var result2 = row.SafeString(1);

        // Assert
        result1.Should().Be("123");
        result2.Should().Be("45.67");
    }

    #endregion

    #region CalculateMortgageMiddleScore Tests

    [Fact]
    public void CalculateMortgageMiddleScore_WithAllThreeScores_ReturnsMiddleValue()
    {
        // Arrange
        var profile = new CreditProfile
        {
            Person = "Test Person",
            Fico5 = "750",
            Fico4 = "740",
            Fico2 = "760"
        };

        // Act
        var result = profile.CalculateMortgageMiddleScore();

        // Assert
        result.Should().Be(750);
    }

    [Fact]
    public void CalculateMortgageMiddleScore_WithScoresInDescendingOrder_ReturnsMiddle()
    {
        // Arrange
        var profile = new CreditProfile
        {
            Person = "Test Person",
            Fico5 = "800",
            Fico4 = "750",
            Fico2 = "700"
        };

        // Act
        var result = profile.CalculateMortgageMiddleScore();

        // Assert
        result.Should().Be(750);
    }

    [Fact]
    public void CalculateMortgageMiddleScore_WithScoresInAscendingOrder_ReturnsMiddle()
    {
        // Arrange
        var profile = new CreditProfile
        {
            Person = "Test Person",
            Fico5 = "700",
            Fico4 = "750",
            Fico2 = "800"
        };

        // Act
        var result = profile.CalculateMortgageMiddleScore();

        // Assert
        result.Should().Be(750);
    }

    [Fact]
    public void CalculateMortgageMiddleScore_WithIdenticalScores_ReturnsThatScore()
    {
        // Arrange
        var profile = new CreditProfile
        {
            Person = "Test Person",
            Fico5 = "750",
            Fico4 = "750",
            Fico2 = "750"
        };

        // Act
        var result = profile.CalculateMortgageMiddleScore();

        // Assert
        result.Should().Be(750);
    }

    [Fact]
    public void CalculateMortgageMiddleScore_WithOneMissingScore_ReturnsNull()
    {
        // Arrange
        var profile = new CreditProfile
        {
            Person = "Test Person",
            Fico5 = "750",
            Fico4 = "740",
            Fico2 = "" // Missing
        };

        // Act
        var result = profile.CalculateMortgageMiddleScore();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CalculateMortgageMiddleScore_WithTwoMissingScores_ReturnsNull()
    {
        // Arrange
        var profile = new CreditProfile
        {
            Person = "Test Person",
            Fico5 = "750",
            Fico4 = "",
            Fico2 = ""
        };

        // Act
        var result = profile.CalculateMortgageMiddleScore();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CalculateMortgageMiddleScore_WithAllMissingScores_ReturnsNull()
    {
        // Arrange
        var profile = new CreditProfile
        {
            Person = "Test Person",
            Fico5 = "",
            Fico4 = "",
            Fico2 = ""
        };

        // Act
        var result = profile.CalculateMortgageMiddleScore();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CalculateMortgageMiddleScore_WithNullScores_ReturnsNull()
    {
        // Arrange
        var profile = new CreditProfile
        {
            Person = "Test Person",
            Fico5 = null!,
            Fico4 = null!,
            Fico2 = null!
        };

        // Act
        var result = profile.CalculateMortgageMiddleScore();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CalculateMortgageMiddleScore_WithInvalidScoreFormat_ReturnsNull()
    {
        // Arrange
        var profile = new CreditProfile
        {
            Person = "Test Person",
            Fico5 = "ABC",
            Fico4 = "750",
            Fico2 = "760"
        };

        // Act
        var result = profile.CalculateMortgageMiddleScore();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CalculateMortgageMiddleScore_WithMixOfValidAndInvalidScores_ReturnsNull()
    {
        // Arrange
        var profile = new CreditProfile
        {
            Person = "Test Person",
            Fico5 = "750",
            Fico4 = "Not a number",
            Fico2 = "760"
        };

        // Act
        var result = profile.CalculateMortgageMiddleScore();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CalculateMortgageMiddleScore_WithEdgeCaseScores_ReturnsMiddle()
    {
        // Arrange
        var profile = new CreditProfile
        {
            Person = "Test Person",
            Fico5 = "300", // Min possible score
            Fico4 = "650",
            Fico2 = "850"  // Max possible score
        };

        // Act
        var result = profile.CalculateMortgageMiddleScore();

        // Assert
        result.Should().Be(650);
    }

    #endregion
}
