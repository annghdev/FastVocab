using FastVocab.Application.Features.Topics.Commands.UpdateTopic;
using FastVocab.Application.Features.Topics.Validators;
using FastVocab.Shared.DTOs.Topics;
using FluentAssertions;

namespace FastVocab.Application.Test.Features.Topics.Validators;

public class UpdateTopicValidatorTests
{
    private readonly UpdateTopicValidator _validator;

    public UpdateTopicValidatorTests()
    {
        _validator = new UpdateTopicValidator();
    }

    [Fact]
    public async Task Validate_WithValidRequest_ShouldPass()
    {
        // Arrange
        var request = new UpdateTopicRequest
        {
            Id = 1,
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            ImageUrl = "https://example.com/image.jpg",
            IsHiding = false
        };
        var command = new UpdateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors?.FirstOrDefault()?.ErrorMessage.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validate_WithInvalidId_ShouldFail(int invalidId)
    {
        // Arrange
        var request = new UpdateTopicRequest
        {
            Id = invalidId,
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            IsHiding = false
        };
        var command = new UpdateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Id");
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.Id")?.ErrorMessage.Should().Contain("greater than 0");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Validate_WithEmptyName_ShouldFail(string name)
    {
        // Arrange
        var request = new UpdateTopicRequest
        {
            Id = 1,
            Name = name!,
            VnText = "Tiếng Anh Thương Mại",
            IsHiding = false
        };
        var command = new UpdateTopicCommand(request);

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
        var request = new UpdateTopicRequest
        {
            Id = 1,
            Name = "A",
            VnText = "Tiếng Anh",
            IsHiding = false
        };
        var command = new UpdateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.Name")?.ErrorMessage.Should().Contain("at least 2 characters");
    }

    [Fact]
    public async Task Validate_WithNameTooLong_ShouldFail()
    {
        // Arrange
        var request = new UpdateTopicRequest
        {
            Id = 1,
            Name = new string('A', 101),
            VnText = "Tiếng Anh",
            IsHiding = false
        };
        var command = new UpdateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.Name")?.ErrorMessage.Should().Contain("must not exceed 100 characters");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Validate_WithEmptyVnText_ShouldFail(string vnText)
    {
        // Arrange
        var request = new UpdateTopicRequest
        {
            Id = 1,
            Name = "Business English",
            VnText = vnText!,
            IsHiding = false
        };
        var command = new UpdateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.VnText");
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.Name")?.ErrorMessage.Should().Contain("required");
    }

    [Fact]
    public async Task Validate_WithNullImageUrl_ShouldPass()
    {
        // Arrange
        var request = new UpdateTopicRequest
        {
            Id = 1,
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            ImageUrl = null,
            IsHiding = false
        };
        var command = new UpdateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_WithImageUrlTooLong_ShouldFail()
    {
        // Arrange
        var request = new UpdateTopicRequest
        {
            Id = 1,
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            ImageUrl = "https://example.com/" + new string('A', 500),
            IsHiding = false
        };
        var command = new UpdateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.Name")?.ErrorMessage.Should().Contain("must not exceed 500 characters");
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com/image.jpg")]
    public async Task Validate_WithInvalidImageUrl_ShouldFail(string invalidUrl)
    {
        // Arrange
        var request = new UpdateTopicRequest
        {
            Id = 1,
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            ImageUrl = invalidUrl,
            IsHiding = false
        };
        var command = new UpdateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.FirstOrDefault(e => e.PropertyName == "Request.Name")?.ErrorMessage.Should().Contain("valid URL");
    }

    [Theory]
    [InlineData("http://example.com/image.jpg")]
    [InlineData("https://example.com/image.png")]
    public async Task Validate_WithValidImageUrl_ShouldPass(string validUrl)
    {
        // Arrange
        var request = new UpdateTopicRequest
        {
            Id = 1,
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            ImageUrl = validUrl,
            IsHiding = false
        };
        var command = new UpdateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Validate_WithAnyIsHidingValue_ShouldPass(bool isHiding)
    {
        // Arrange
        var request = new UpdateTopicRequest
        {
            Id = 1,
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            IsHiding = isHiding
        };
        var command = new UpdateTopicCommand(request);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}

