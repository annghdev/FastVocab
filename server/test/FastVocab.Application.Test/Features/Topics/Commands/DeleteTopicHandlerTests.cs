using FastVocab.Application.Features.Topics.Commands.DeleteTopic;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace FastVocab.Application.Test.Features.Topics.Commands;

public class DeleteTopicHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteTopicHandler _handler;

    public DeleteTopicHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeleteTopicHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidTopic_ShouldSoftDeleteSuccessfully()
    {
        // Arrange
        var topicId = 1;
        var command = new DeleteTopicCommand(topicId);

        var topic = new Topic
        {
            Id = topicId,
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            IsDeleted = false
        };

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(topicId))
            .ReturnsAsync(topic);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        topic.IsDeleted.Should().BeTrue();

        _unitOfWorkMock.Verify(x => x.Topics.Update(topic), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentTopic_ShouldReturnFailure()
    {
        // Arrange
        var topicId = 999;
        var command = new DeleteTopicCommand(topicId);

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(topicId))
            .ReturnsAsync((Topic?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("not found");

        _unitOfWorkMock.Verify(x => x.Topics.Update(It.IsAny<Topic>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithAlreadyDeletedTopic_ShouldReturnFailure()
    {
        // Arrange
        var topicId = 1;
        var command = new DeleteTopicCommand(topicId);

        var topic = new Topic
        {
            Id = topicId,
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            IsDeleted = true // Already deleted
        };

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(topicId))
            .ReturnsAsync(topic);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("already been deleted");

        _unitOfWorkMock.Verify(x => x.Topics.Update(It.IsAny<Topic>()), Times.Never);
    }
}

