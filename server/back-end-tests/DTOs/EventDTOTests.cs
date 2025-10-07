using back_end.DTOs;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace back_end_tests.DTOs
{
    public class EventDTOTests
    {
        [Fact]
        public void EventDTO_DefaultConstructor_SetsDefaultValues()
        {
            // Arrange & Act
            var dto = new EventDTO();

            // Assert
            Assert.Equal(0, dto.Id);
            Assert.Equal("", dto.Title);
            Assert.Equal("", dto.Location);
            Assert.Equal("", dto.City);
            Assert.Equal("", dto.ImageUrl);
            Assert.Equal("", dto.Url);
            Assert.Equal(0, dto.Price);
            Assert.Equal(0, dto.HostId);
            Assert.Equal(0, dto.Status);
            Assert.Null(dto.MinAge);
            Assert.Null(dto.MaxAge);
            Assert.Null(dto.DisabilityTags);
        }

        [Fact]
        public void EventDTO_SetProperties_PropertiesAreSetCorrectly()
        {
            // Arrange
            var dto = new EventDTO();
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = DateTime.UtcNow.AddDays(1).AddHours(2);
            var tags = new List<string> { "Wheelchair Accessible", "ASL Interpreter" };

            // Act
            dto.Id = 1;
            dto.Title = "Test Event";
            dto.Location = "123 Test St";
            dto.City = "Toronto";
            dto.ImageUrl = "https://test.com/image.jpg";
            dto.StartDateTime = startDate;
            dto.EndDateTime = endDate;
            dto.Price = 50.99m;
            dto.Url = "https://test.com";
            dto.HostId = 1;
            dto.MinAge = 18;
            dto.MaxAge = 65;
            dto.DisabilityTags = tags;
            dto.Status = 0;

            // Assert
            Assert.Equal(1, dto.Id);
            Assert.Equal("Test Event", dto.Title);
            Assert.Equal("123 Test St", dto.Location);
            Assert.Equal("Toronto", dto.City);
            Assert.Equal("https://test.com/image.jpg", dto.ImageUrl);
            Assert.Equal(startDate, dto.StartDateTime);
            Assert.Equal(endDate, dto.EndDateTime);
            Assert.Equal(50.99m, dto.Price);
            Assert.Equal("https://test.com", dto.Url);
            Assert.Equal(1, dto.HostId);
            Assert.Equal(18, dto.MinAge);
            Assert.Equal(65, dto.MaxAge);
            Assert.Equal(tags, dto.DisabilityTags);
            Assert.Equal(0, dto.Status);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void EventDTO_RequiredFields_ValidationFailsForInvalidValues(string invalidValue)
        {
            // Arrange
            var dto = new EventDTO
            {
                Title = invalidValue,
                Location = invalidValue,
                City = invalidValue,
                StartDateTime = DateTime.UtcNow.AddDays(1),
                EndDateTime = DateTime.UtcNow.AddDays(2)
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(EventDTO.Title)));
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(EventDTO.Location)));
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(EventDTO.City)));
        }

        [Fact]
        public void EventDTO_ValidModel_PassesValidation()
        {
            // Arrange
            var dto = new EventDTO
            {
                Title = "Valid Event",
                Location = "Valid Location",
                City = "Toronto",
                StartDateTime = DateTime.UtcNow.AddDays(1),
                EndDateTime = DateTime.UtcNow.AddDays(1).AddHours(2),
                Price = 25.50m,
                Url = "https://valid-url.com"
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.Empty(validationResults);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100.50)]
        public void EventDTO_NegativePrice_ValidationFails(decimal negativePrice)
        {
            // Arrange
            var dto = new EventDTO
            {
                Title = "Test Event",
                Location = "Test Location",
                City = "Toronto",
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(1),
                Price = negativePrice
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(EventDTO.Price)));
        }

        [Theory]
        [InlineData("invalid-url")]
        [InlineData("not a url")]
        public void EventDTO_InvalidUrl_ValidationFails(string invalidUrl)
        {
            // Arrange
            var dto = new EventDTO
            {
                Title = "Test Event",
                Location = "Test Location",
                City = "Toronto",
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(1),
                Url = invalidUrl
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(EventDTO.Url)));
        }

        [Theory]
        [InlineData("https://valid-url.com")]
        [InlineData("http://also-valid.com")]
        public void EventDTO_ValidUrl_PassesValidation(string validUrl)
        {
            // Arrange
            var dto = new EventDTO
            {
                Title = "Test Event",
                Location = "Test Location",
                City = "Toronto",
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(1),
                Url = validUrl
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains(nameof(EventDTO.Url)));
        }

        [Theory]
        [InlineData(151)]
        [InlineData(200)]
        [InlineData(-1)]
        public void EventDTO_InvalidMinAge_ValidationFails(int invalidAge)
        {
            // Arrange
            var dto = new EventDTO
            {
                Title = "Test Event",
                Location = "Test Location",
                City = "Toronto",
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(1),
                MinAge = invalidAge
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(EventDTO.MinAge)));
        }

        [Theory]
        [InlineData(151)]
        [InlineData(200)]
        [InlineData(-1)]
        public void EventDTO_InvalidMaxAge_ValidationFails(int invalidAge)
        {
            // Arrange
            var dto = new EventDTO
            {
                Title = "Test Event",
                Location = "Test Location",
                City = "Toronto",
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(1),
                MaxAge = invalidAge
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(EventDTO.MaxAge)));
        }

        [Fact]
        public void EventDTO_TitleExceedsMaxLength_ValidationFails()
        {
            // Arrange
            var dto = new EventDTO
            {
                Title = new string('A', 201), // Max length is 200
                Location = "Test Location",
                City = "Toronto",
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(1)
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(EventDTO.Title)));
        }

        [Fact]
        public void EventDTO_LocationExceedsMaxLength_ValidationFails()
        {
            // Arrange
            var dto = new EventDTO
            {
                Title = "Test Event",
                Location = new string('A', 301), // Max length is 300
                City = "Toronto",
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(1)
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(EventDTO.Location)));
        }

        [Fact]
        public void EventDTO_DisabilityTags_CanBeNull()
        {
            // Arrange
            var dto = new EventDTO
            {
                Title = "Test Event",
                Location = "Test Location",
                City = "Toronto",
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(1),
                DisabilityTags = null
            };

            // Act
            var validationResults = ValidateModel(dto);

            // Assert - No validation errors for null tags
            Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains(nameof(EventDTO.DisabilityTags)));
        }

        [Fact]
        public void EventDTO_DisabilityTags_CanBeEmptyList()
        {
            // Arrange
            var dto = new EventDTO
            {
                Title = "Test Event",
                Location = "Test Location",
                City = "Toronto",
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(1),
                DisabilityTags = new List<string>()
            };

            // Assert
            Assert.NotNull(dto.DisabilityTags);
            Assert.Empty(dto.DisabilityTags);
        }

        private static IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
    }
}


