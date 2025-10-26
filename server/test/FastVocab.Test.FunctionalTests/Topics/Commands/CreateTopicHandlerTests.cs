using AutoMapper;
using FastVocab.Application.Features.Topics.Commands.CreateTopic;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Topics;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace FastVocab.Test.FunctionalTests.Topics.Commands;

public class CreateTopicHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateTopicHandler _handler;

    public CreateTopicHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateTopicHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateTopicSuccessfully()
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            ImageUrl = "https://example.com/image.jpg"
        };
        var command = new CreateTopicCommand(request);

        var topic = new Topic
        {
            Id = 1,
            Name = request.Name,
            VnText = request.VnText,
            ImageUrl = request.ImageUrl,
            IsHiding = false
        };

        var topicDto = new TopicDto
        {
            Id = 1,
            Name = request.Name,
            VnText = request.VnText,
            ImageUrl = request.ImageUrl,
            IsHiding = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Setup mocks
        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(It.IsAny<Expression<Func<Topic, bool>>>()))
            .ReturnsAsync((Topic?)null); // No existing topic

        _mapperMock.Setup(x => x.Map<Topic>(request))
            .Returns(topic);

        _unitOfWorkMock.Setup(x => x.Topics.Add(It.IsAny<Topic>()))
            .Returns(topic);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapperMock.Setup(x => x.Map<TopicDto>(It.IsAny<Topic>()))
            .Returns(topicDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(request.Name);
        result.Data.VnText.Should().Be(request.VnText);
        result.Data.IsHiding.Should().BeFalse();

        _unitOfWorkMock.Verify(x => x.Topics.Add(It.IsAny<Topic>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDuplicateName_ShouldReturnFailure()
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại"
        };
        var command = new CreateTopicCommand(request);

        var existingTopic = new Topic
        {
            Id = 1,
            Name = request.Name,
            VnText = "Old VnText"
        };

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(It.IsAny<Expression<Func<Topic, bool>>>()))
            .ReturnsAsync(existingTopic);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("already exists");

        _unitOfWorkMock.Verify(x => x.Topics.Add(It.IsAny<Topic>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSetIsHidingToFalseByDefault()
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = "Technology",
            VnText = "Công Nghệ"
        };
        var command = new CreateTopicCommand(request);

        Topic? capturedTopic = null;

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(It.IsAny<Expression<Func<Topic, bool>>>()))
            .ReturnsAsync((Topic?)null);

        _mapperMock.Setup(x => x.Map<Topic>(request))
            .Returns(new Topic { Name = request.Name, VnText = request.VnText });

        _unitOfWorkMock.Setup(x => x.Topics.Add(It.IsAny<Topic>()))
            .Callback<Topic>(t => capturedTopic = t)
            .Returns<Topic>(t => t);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapperMock.Setup(x => x.Map<TopicDto>(It.IsAny<Topic>()))
            .Returns(new TopicDto { Id = 1, Name = request.Name, VnText = request.VnText, IsHiding = false, CreatedAt = DateTimeOffset.UtcNow });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedTopic.Should().NotBeNull();
        capturedTopic!.IsHiding.Should().BeFalse();
    }
}

