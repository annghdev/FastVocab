using FastVocab.Application.Features.Words.Commands.UpdateWord;
using FastVocab.Application.Features.Words.Validators;
using FastVocab.Shared.DTOs.Words;
using FluentAssertions;

namespace FastVocab.Application.Test.Features.Words.Validators;

public class UpdateWordValidatorTests
{
    private readonly UpdateWordValidator _validator;

    public UpdateWordValidatorTests()
    {
        _validator = new UpdateWordValidator();
    }

    [Fact]
    public async Task Validate_WithValidRequest_ShouldPass()
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = 1,
            Text = "perseverance",
            Meaning = "sự kiên trì",
            Type = "Noun",
            Level = "B2"
        };
        var command = new UpdateWordCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors?.FirstOrDefault()?.ErrorMessage.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WithInvalidId_ShouldFail(int invalidId)
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = invalidId,
            Text = "test",
            Meaning = "nghĩa",
            Type = "Noun",
            Level = "A1"
        };
        var command = new UpdateWordCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Id");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_WithEmptyText_ShouldFail(string text)
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = 1,
            Text = text,
            Meaning = "nghĩa",
            Type = "Noun",
            Level = "A1"
        };
        var command = new UpdateWordCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Text");
    }

    [Fact]
    public async Task Validate_WithTextTooLong_ShouldFail()
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = 1,
            Text = new string('a', 101),
            Meaning = "nghĩa",
            Type = "Noun",
            Level = "A1"
        };
        var command = new UpdateWordCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.Text")?.ErrorMessage.Should().Contain("100 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_WithEmptyMeaning_ShouldFail(string meaning)
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = 1,
            Text = "test",
            Meaning = meaning,
            Type = "Noun",
            Level = "A1"
        };
        var command = new UpdateWordCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Meaning");
    }

    [Fact]
    public async Task Validate_WithMeaningTooLong_ShouldFail()
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = 1,
            Text = "test",
            Meaning = new string('a', 501),
            Type = "Noun",
            Level = "A1"
        };
        var command = new UpdateWordCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.Meaning")?.ErrorMessage.Should().Contain("500 characters");
    }

    [Theory]
    [InlineData("InvalidType")]
    [InlineData("Random")]
    public async Task Validate_WithInvalidType_ShouldFail(string invalidType)
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = 1,
            Text = "test",
            Meaning = "nghĩa",
            Type = invalidType,
            Level = "A1"
        };
        var command = new UpdateWordCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.Type")?.ErrorMessage.Should().Contain("Word type must be one of");
    }

    [Theory]
    [InlineData("X1")]
    [InlineData("D1")]
    public async Task Validate_WithInvalidLevel_ShouldFail(string invalidLevel)
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = 1,
            Text = "test",
            Meaning = "nghĩa",
            Type = "Noun",
            Level = invalidLevel
        };
        var command = new UpdateWordCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.Level")?.ErrorMessage.Should().Contain("Word level must be one of");
    }

    [Theory]
    [InlineData("Noun")]
    [InlineData("Verb")]
    [InlineData("Adjective")]
    [InlineData("Adverb")]
    [InlineData("Pronoun")]
    [InlineData("Preposition")]
    [InlineData("Conjunction")]
    [InlineData("Article")]
    public async Task Validate_WithValidTypes_ShouldPass(string validType)
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = 1,
            Text = "test",
            Meaning = "nghĩa",
            Type = validType,
            Level = "A1"
        };
        var command = new UpdateWordCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("A1")]
    [InlineData("A2")]
    [InlineData("B1")]
    [InlineData("B2")]
    [InlineData("C1")]
    [InlineData("C2")]
    public async Task Validate_WithValidLevels_ShouldPass(string validLevel)
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = 1,
            Text = "test",
            Meaning = "nghĩa",
            Type = "Noun",
            Level = validLevel
        };
        var command = new UpdateWordCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com")]
    public async Task Validate_WithInvalidImageUrl_ShouldFail(string invalidUrl)
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = 1,
            Text = "test",
            Meaning = "nghĩa",
            Type = "Noun",
            Level = "A1",
            ImageUrl = invalidUrl
        };
        var command = new UpdateWordCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.ImageUrl")?.ErrorMessage.Should().Contain("valid URL");
    }

    [Theory]
    [InlineData("http://example.com/image.jpg")]
    [InlineData("https://example.com/image.png")]
    public async Task Validate_WithValidImageUrl_ShouldPass(string validUrl)
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = 1,
            Text = "test",
            Meaning = "nghĩa",
            Type = "Noun",
            Level = "A1",
            ImageUrl = validUrl
        };
        var command = new UpdateWordCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithAllOptionalFields_ShouldPass()
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = 1,
            Text = "comprehensive",
            Meaning = "toàn diện",
            Type = "Adjective",
            Level = "B2",
            Definition = "including all or nearly all elements",
            Example1 = "A comprehensive guide",
            Example2 = "Comprehensive coverage",
            Example3 = "A comprehensive study",
            ImageUrl = "https://example.com/image.jpg",
            AudioUrl = "https://example.com/audio.mp3"
        };
        var command = new UpdateWordCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}

