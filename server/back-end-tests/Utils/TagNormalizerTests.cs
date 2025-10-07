using Xunit;
using back_end.Utils;

namespace back_end_tests.Utils
{
    public class TagNormalizerTests
    {
        [Fact]
        public void Normalize_WithValidString_ShouldReturnUppercaseTrimmedString()
        {
            // Arrange
            var input = "Wheelchair Accessible";

            // Act
            var result = TagNormalizer.Normalize(input);

            // Assert
            Assert.Equal("WHEELCHAIR ACCESSIBLE", result);
        }

        [Fact]
        public void Normalize_WithLowercaseString_ShouldReturnUppercaseString()
        {
            // Arrange
            var input = "wheelchair accessible";

            // Act
            var result = TagNormalizer.Normalize(input);

            // Assert
            Assert.Equal("WHEELCHAIR ACCESSIBLE", result);
        }

        [Fact]
        public void Normalize_WithMixedCaseString_ShouldReturnUppercaseString()
        {
            // Arrange
            var input = "WheElChAiR AcCeSsIbLe";

            // Act
            var result = TagNormalizer.Normalize(input);

            // Assert
            Assert.Equal("WHEELCHAIR ACCESSIBLE", result);
        }

        [Fact]
        public void Normalize_WithLeadingWhitespace_ShouldTrimAndReturnUppercase()
        {
            // Arrange
            var input = "   Wheelchair Accessible";

            // Act
            var result = TagNormalizer.Normalize(input);

            // Assert
            Assert.Equal("WHEELCHAIR ACCESSIBLE", result);
        }

        [Fact]
        public void Normalize_WithTrailingWhitespace_ShouldTrimAndReturnUppercase()
        {
            // Arrange
            var input = "Wheelchair Accessible   ";

            // Act
            var result = TagNormalizer.Normalize(input);

            // Assert
            Assert.Equal("WHEELCHAIR ACCESSIBLE", result);
        }

        [Fact]
        public void Normalize_WithLeadingAndTrailingWhitespace_ShouldTrimAndReturnUppercase()
        {
            // Arrange
            var input = "   Wheelchair Accessible   ";

            // Act
            var result = TagNormalizer.Normalize(input);

            // Assert
            Assert.Equal("WHEELCHAIR ACCESSIBLE", result);
        }

        [Fact]
        public void Normalize_WithEmptyString_ShouldReturnEmptyString()
        {
            // Arrange
            var input = "";

            // Act
            var result = TagNormalizer.Normalize(input);

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void Normalize_WithWhitespaceOnly_ShouldReturnEmptyString()
        {
            // Arrange
            var input = "   ";

            // Act
            var result = TagNormalizer.Normalize(input);

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void Normalize_WithNull_ShouldReturnEmptyString()
        {
            // Arrange
            string input = null;

            // Act
            var result = TagNormalizer.Normalize(input);

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void Normalize_WithSpecialCharacters_ShouldPreserveAndUppercase()
        {
            // Arrange
            var input = "ASL/Sign Language";

            // Act
            var result = TagNormalizer.Normalize(input);

            // Assert
            Assert.Equal("ASL/SIGN LANGUAGE", result);
        }

        [Fact]
        public void Normalize_WithNumbers_ShouldPreserveNumbers()
        {
            // Arrange
            var input = "Type 1 Diabetes";

            // Act
            var result = TagNormalizer.Normalize(input);

            // Assert
            Assert.Equal("TYPE 1 DIABETES", result);
        }

        [Fact]
        public void Normalize_SameValuesDifferentCasing_ShouldReturnSameNormalized()
        {
            // Arrange
            var input1 = "Wheelchair Accessible";
            var input2 = "wheelchair accessible";
            var input3 = "WHEELCHAIR ACCESSIBLE";

            // Act
            var result1 = TagNormalizer.Normalize(input1);
            var result2 = TagNormalizer.Normalize(input2);
            var result3 = TagNormalizer.Normalize(input3);

            // Assert
            Assert.Equal(result1, result2);
            Assert.Equal(result2, result3);
            Assert.Equal(result1, result3);
        }

        [Fact]
        public void Normalize_SameValuesWithWhitespace_ShouldReturnSameNormalized()
        {
            // Arrange
            var input1 = "Wheelchair Accessible";
            var input2 = "  Wheelchair Accessible  ";
            var input3 = "Wheelchair Accessible   ";

            // Act
            var result1 = TagNormalizer.Normalize(input1);
            var result2 = TagNormalizer.Normalize(input2);
            var result3 = TagNormalizer.Normalize(input3);

            // Assert
            Assert.Equal(result1, result2);
            Assert.Equal(result2, result3);
            Assert.Equal(result1, result3);
        }

        [Theory]
        [InlineData("ASL Interpreter", "ASL INTERPRETER")]
        [InlineData("Closed Captioning", "CLOSED CAPTIONING")]
        [InlineData("Braille Materials", "BRAILLE MATERIALS")]
        [InlineData("Service Animal Friendly", "SERVICE ANIMAL FRIENDLY")]
        public void Normalize_WithVariousInputs_ShouldReturnExpectedOutput(string input, string expected)
        {
            // Act
            var result = TagNormalizer.Normalize(input);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}


