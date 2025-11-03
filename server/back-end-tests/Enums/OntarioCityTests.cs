using back_end.Enums;
using Xunit;

namespace back_end_tests.Enums
{
    public class OntarioCityTests
    {
        [Theory]
        [InlineData("Toronto")]
        [InlineData("Ottawa")]
        [InlineData("Hamilton")]
        [InlineData("Kitchener")]
        [InlineData("London")]
        [InlineData("Windsor")]
        [InlineData("Mississauga")]
        [InlineData("Brampton")]
        public void OntarioCity_MajorCities_ExistInEnum(string cityName)
        {
            // Act
            var canParse = Enum.TryParse<OntarioCity>(cityName, out var city);

            // Assert
            Assert.True(canParse);
            Assert.Equal(cityName, city.ToString());
        }

        [Fact]
        public void OntarioCity_HasToronto()
        {
            // Act & Assert
            Assert.Equal("Toronto", OntarioCity.Toronto.ToString());
        }

        [Fact]
        public void OntarioCity_HasOttawa()
        {
            // Act & Assert
            Assert.Equal("Ottawa", OntarioCity.Ottawa.ToString());
        }

        [Fact]
        public void OntarioCity_HasHamilton()
        {
            // Act & Assert
            Assert.Equal("Hamilton", OntarioCity.Hamilton.ToString());
        }

        [Theory]
        [InlineData("Ajax")]
        [InlineData("Aurora")]
        [InlineData("Barrie")]
        [InlineData("Belleville")]
        [InlineData("Burlington")]
        [InlineData("Cambridge")]
        [InlineData("Cornwall")]
        [InlineData("Guelph")]
        [InlineData("Kingston")]
        [InlineData("Markham")]
        [InlineData("Milton")]
        [InlineData("Oakville")]
        [InlineData("Oshawa")]
        [InlineData("Peterborough")]
        [InlineData("Pickering")]
        [InlineData("Sarnia")]
        [InlineData("Sudbury")]
        [InlineData("ThunderBay")]
        [InlineData("Vaughan")]
        [InlineData("Waterloo")]
        [InlineData("Whitby")]
        public void OntarioCity_AllCities_CanBeParsed(string cityName)
        {
            // Act
            var canParse = Enum.TryParse<OntarioCity>(cityName, out var city);

            // Assert
            Assert.True(canParse, $"City '{cityName}' should be parseable");
        }

        [Theory]
        [InlineData("InvalidCity")]
        [InlineData("NewYork")]
        [InlineData("Montreal")]
        public void OntarioCity_InvalidCity_CannotBeParsed(string cityName)
        {
            // Act
            var canParse = Enum.TryParse<OntarioCity>(cityName, out _);

            // Assert
            Assert.False(canParse, $"City '{cityName}' should not be parseable");
        }

        [Fact]
        public void OntarioCity_CaseInsensitiveParsing_Works()
        {
            // Act
            var canParse = Enum.TryParse<OntarioCity>("toronto", true, out var city);

            // Assert
            Assert.True(canParse);
            Assert.Equal(OntarioCity.Toronto, city);
        }

        [Fact]
        public void OntarioCity_AllValuesAreUnique()
        {
            // Arrange
            var allCities = Enum.GetValues<OntarioCity>();

            // Act
            var uniqueCities = allCities.Distinct();

            // Assert
            Assert.Equal(allCities.Length, uniqueCities.Count());
        }

        [Fact]
        public void OntarioCity_HasExpectedNumberOfCities()
        {
            // Arrange & Act
            var cityCount = Enum.GetValues<OntarioCity>().Length;

            // Assert - Based on the enum definition
            Assert.True(cityCount >= 50, $"Expected at least 50 cities, but found {cityCount}");
        }
    }
}


