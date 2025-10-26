using FastVocab.Application.Features.Words.Commands.RemoveWordFromTopic;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Entities.JunctionEntities;
using FastVocab.Domain.Repositories;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace FastVocab.Test.FunctionalTests.Words.Commands;

public class RemoveWordFromTopicHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RemoveWordFromTopicHandler _handler;

    public RemoveWordFromTopicHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new RemoveWordFromTopicHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidAssociation_ShouldRemoveSuccessfully()
    {
        // Arrange
        var wordId = 1;
        var topicId = 1;
        var command = new RemoveWordFromTopicCommand(wordId, topicId);

        var wordTopic = new WordTopic { WordId = wordId, TopicId = topicId };

        var word = new Word
        {
            Id = wordId,
            Text = "test",
            IsDeleted = false,
            Topics = new List<WordTopic> { wordTopic }
        };

        var topic = new Topic
        {
            Id = topicId,
            Name = "Test Topic",
            IsDeleted = false
        };

        _unitOfWorkMock.Setup(x => x.Words.GetWithInfoAsync(
            It.IsAny<Expression<Func<Word, bool>>>(),
            It.IsAny<Expression<Func<Word, object>>>()))
            .ReturnsAsync(word);

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(topicId))
            .ReturnsAsync(topic);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        word.Topics.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithNonExistentWord_ShouldReturnFailure()
    {
        // Arrange
        var command = new RemoveWordFromTopicCommand(999, 1);

        _unitOfWorkMock.Setup(x => x.Words.GetWithInfoAsync(
            It.IsAny<Expression<Func<Word, bool>>>(),
            It.IsAny<Expression<Func<Word, object>>>()))
            .ReturnsAsync((Word?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_WithNonExistentTopic_ShouldReturnFailure()
    {
        // Arrange
        var command = new RemoveWordFromTopicCommand(1, 999);

        var word = new Word
        {
            Id = 1,
            Text = "test",
            IsDeleted = false,
            Topics = new List<WordTopic>()
        };

        _unitOfWorkMock.Setup(x => x.Words.GetWithInfoAsync(
            It.IsAny<Expression<Func<Word, bool>>>(),
            It.IsAny<Expression<Func<Word, object>>>()))
            .ReturnsAsync(word);

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(999))
            .ReturnsAsync((Topic?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_WithNonExistentAssociation_ShouldReturnFailure()
    {
        // Arrange
        var wordId = 1;
        var topicId = 1;
        var command = new RemoveWordFromTopicCommand(wordId, topicId);

        var word = new Word
        {
            Id = wordId,
            Text = "test",
            IsDeleted = false,
            Topics = new List<WordTopic>()
        };

        var topic = new Topic
        {
            Id = topicId,
            Name = "Test Topic",
            IsDeleted = false
        };

        _unitOfWorkMock.Setup(x => x.Words.GetWithInfoAsync(
            It.IsAny<Expression<Func<Word, bool>>>(),
            It.IsAny<Expression<Func<Word, object>>>()))
            .ReturnsAsync(word);

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(topicId))
            .ReturnsAsync(topic);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Be("Operation conflicts");
    }
}

