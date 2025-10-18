using FastVocab.Application.Features.Topics.Commands.CreateTopic;
using FastVocab.Application.Features.Topics.Validators;
using FastVocab.Shared.DTOs.Topics;
using FluentAssertions;

namespace FastVocab.Application.Test.Features.Topics.Validators;

public class CreateTopicValidatorTests
{
    private readonly CreateTopicValidator _validator;

    public CreateTopicValidatorTests()
    {
        _validator = new CreateTopicValidator();
    }

    [Fact]
    public async Task Validate_WithValidRequest_ShouldPass()
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            ImageUrl = "https://example.com/image.jpg"
        };
        var command = new CreateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors?.FirstOrDefault()?.ErrorMessage.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Validate_WithEmptyName_ShouldFail(string name)
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = name!,
            VnText = "Tiếng Anh Thương Mại"
        };
        var command = new CreateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Name");
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.Name")?.ErrorMessage.Should().Contain("required");
    }

    [Fact]
    public async Task Validate_WithNameTooShort_ShouldFail()
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = "A",
            VnText = "Tiếng Anh"
        };
        var command = new CreateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Name");
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.Name")?.ErrorMessage.Should().Contain("at least 2 characters");
    }

    [Fact]
    public async Task Validate_WithNameTooLong_ShouldFail()
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = new string('A', 101), // 101 characters
            VnText = "Tiếng Anh"
        };
        var command = new CreateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Name");
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.Name")?.ErrorMessage.Should().Contain("must not exceed 100 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Validate_WithEmptyVnText_ShouldFail(string vnText)
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = "Business English",
            VnText = vnText!
        };
        var command = new CreateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.VnText");
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.Name")?.ErrorMessage.Should().Contain("required");
    }

    [Fact]
    public async Task Validate_WithVnTextTooShort_ShouldFail()
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = "Business English",
            VnText = "A"
        };
        var command = new CreateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.VnText");
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.Name")?.ErrorMessage.Should().Contain("at least 2 characters");
    }

    [Fact]
    public async Task Validate_WithVnTextTooLong_ShouldFail()
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = "Business English",
            VnText = new string('A', 101) // 101 characters
        };
        var command = new CreateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.VnText");
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.Name")?.ErrorMessage.Should().Contain("must not exceed 100 characters");
    }

    [Fact]
    public async Task Validate_WithNullImageUrl_ShouldPass()
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            ImageUrl = null
        };
        var command = new CreateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithImageUrlTooLong_ShouldFail()
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            ImageUrl = "https://example.com/" + new string('A', 500) // > 500 characters
        };
        var command = new CreateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.ImageUrl");
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.Name")?.ErrorMessage.Should().Contain("must not exceed 500 characters");
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com/image.jpg")]
    [InlineData("just-text")]
    public async Task Validate_WithInvalidImageUrl_ShouldFail(string invalidUrl)
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            ImageUrl = invalidUrl
        };
        var command = new CreateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.ImageUrl");
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.Name")?.ErrorMessage.Should().Contain("valid URL");
    }

    [Theory]
    [InlineData("http://example.com/image.jpg")]
    [InlineData("https://example.com/image.png")]
    [InlineData("https://cdn.example.com/path/to/image.jpg")]
    public async Task Validate_WithValidImageUrl_ShouldPass(string validUrl)
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            ImageUrl = validUrl
        };
        var command = new CreateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}

