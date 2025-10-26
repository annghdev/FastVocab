using AutoMapper;
using FastVocab.Application.Features.Topics.Commands.ToggleTopicVisibility;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Topics;
using FluentAssertions;
using Moq;

namespace FastVocab.Test.FunctionalTests.Topics.Commands;

public class ToggleTopicVisibilityHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ToggleTopicVisibilityHandler _handler;

    public ToggleTopicVisibilityHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new ToggleTopicVisibilityHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WithVisibleTopic_ShouldHideTopic()
    {
        // Arrange
        var topicId = 1;
        var command = new ToggleTopicVisibilityCommand(topicId);

        var topic = new Topic
        {
            Id = topicId,
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            IsHiding = false, // Currently visible
            IsDeleted = false
        };

        var topicDto = new TopicDto
        {
            Id = topicId,
            Name = topic.Name,
            VnText = topic.VnText,
            IsHiding = true, // After toggle
            CreatedAt = DateTimeOffset.UtcNow
        };

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(topicId))
            .ReturnsAsync(topic);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapperMock.Setup(x => x.Map<TopicDto>(It.IsAny<Topic>()))
            .Returns(topicDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.IsHiding.Should().BeTrue();
        topic.IsHiding.Should().BeTrue();

        _unitOfWorkMock.Verify(x => x.Topics.Update(topic), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithHiddenTopic_ShouldShowTopic()
    {
        // Arrange
        var topicId = 1;
        var command = new ToggleTopicVisibilityCommand(topicId);

        var topic = new Topic
        {
            Id = topicId,
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            IsHiding = true, // Currently hidden
            IsDeleted = false
        };

        var topicDto = new TopicDto
        {
            Id = topicId,
            Name = topic.Name,
            VnText = topic.VnText,
            IsHiding = false, // After toggle
            CreatedAt = DateTimeOffset.UtcNow
        };

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(topicId))
            .ReturnsAsync(topic);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapperMock.Setup(x => x.Map<TopicDto>(It.IsAny<Topic>()))
            .Returns(topicDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.IsHiding.Should().BeFalse();
        topic.IsHiding.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithNonExistentTopic_ShouldReturnFailure()
    {
        // Arrange
        var topicId = 999;
        var command = new ToggleTopicVisibilityCommand(topicId);

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(topicId))
            .ReturnsAsync((Topic?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("not found");

        _unitOfWorkMock.Verify(x => x.Topics.Update(It.IsAny<Topic>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithDeletedTopic_ShouldReturnFailure()
    {
        // Arrange
        var topicId = 1;
        var command = new ToggleTopicVisibilityCommand(topicId);

        var topic = new Topic
        {
            Id = topicId,
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            IsDeleted = true
        };

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(topicId))
            .ReturnsAsync(topic);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("deleted");

        _unitOfWorkMock.Verify(x => x.Topics.Update(It.IsAny<Topic>()), Times.Never);
    }
}

