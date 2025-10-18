using AutoMapper;
using FastVocab.Application.Features.Words.Commands.UpdateWord;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Entities.JunctionEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Words;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace FastVocab.Application.Test.Features.Words.Commands;

public class UpdateWordHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateWordHandler _handler;

    public UpdateWordHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateWordHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldUpdateWordSuccessfully()
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = 1,
            Text = "perseverance",
            Meaning = "sự kiên trì, kiên nhẫn",
            Type = "Noun",
            Level = "B2",
            Definition = "continued effort despite difficulties"
        };
        var command = new UpdateWordCommand(request);

        var word = new Word
        {
            Id = 1,
            Text = "perseverance",
            Meaning = "sự kiên trì",
            Type = "Noun",
            Level = "B2",
            IsDeleted = false,
            Topics = new List<WordTopic>()
        };

        _unitOfWorkMock.Setup(x => x.Words.GetWithInfoAsync(
            It.IsAny<Expression<Func<Word, bool>>>(),
            It.IsAny<Expression<Func<Word, object>>>()))
            .ReturnsAsync(word);

        _unitOfWorkMock.Setup(x => x.Words.FindAsync(It.IsAny<Expression<Func<Word, bool>>>()))
            .ReturnsAsync((Word?)null);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var wordDto = new WordDto
        {
            Id = 1,
            Text = request.Text,
            Meaning = request.Meaning,
            Type = request.Type,
            Level = request.Level,
            Definition = request.Definition,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _mapperMock.Setup(x => x.Map<WordDto>(It.IsAny<Word>()))
            .Returns(wordDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Meaning.Should().Be(request.Meaning);

        _unitOfWorkMock.Verify(x => x.Words.Update(word), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentWord_ShouldReturnFailure()
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = 999,
            Text = "test",
            Meaning = "test",
            Type = "Noun",
            Level = "A1"
        };
        var command = new UpdateWordCommand(request);

        _unitOfWorkMock.Setup(x => x.Words.GetWithInfoAsync(
            It.IsAny<Expression<Func<Word, bool>>>(),
            It.IsAny<Expression<Func<Word, object>>>()))
            .ReturnsAsync((Word?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("not found");

        _unitOfWorkMock.Verify(x => x.Words.Update(It.IsAny<Word>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithDeletedWord_ShouldReturnFailure()
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = 1,
            Text = "test",
            Meaning = "test",
            Type = "Noun",
            Level = "A1"
        };
        var command = new UpdateWordCommand(request);

        var word = new Word
        {
            Id = 1,
            Text = "test",
            IsDeleted = true
        };

        _unitOfWorkMock.Setup(x => x.Words.GetWithInfoAsync(
            It.IsAny<Expression<Func<Word, bool>>>(),
            It.IsAny<Expression<Func<Word, object>>>()))
            .ReturnsAsync(word);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("been deleted");

        _unitOfWorkMock.Verify(x => x.Words.Update(It.IsAny<Word>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithTopicIds_ShouldUpdateTopicAssociations()
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = 1,
            Text = "test",
            Meaning = "test",
            Type = "Noun",
            Level = "A1",
            TopicIds = new List<int> { 1, 2 }
        };
        var command = new UpdateWordCommand(request);

        var word = new Word
        {
            Id = 1,
            Text = "test",
            Meaning = "test",
            IsDeleted = false,
            Topics = new List<WordTopic>()
        };

        var topic1 = new Topic { Id = 1, Name = "Topic1", IsDeleted = false };
        var topic2 = new Topic { Id = 2, Name = "Topic2", IsDeleted = false };

        _unitOfWorkMock.Setup(x => x.Words.GetWithInfoAsync(
            It.IsAny<Expression<Func<Word, bool>>>(),
            It.IsAny<Expression<Func<Word, object>>>()))
            .ReturnsAsync(word);

        _unitOfWorkMock.Setup(x => x.Words.FindAsync(It.IsAny<Expression<Func<Word, bool>>>()))
            .ReturnsAsync((Word?)null);

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(1))
            .ReturnsAsync(topic1);

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(2))
            .ReturnsAsync(topic2);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapperMock.Setup(x => x.Map<WordDto>(It.IsAny<Word>()))
            .Returns(new WordDto
            {
                Id = 1,
                Text = "test",
                Meaning = "test",
                Type = "Noun",
                Level = "A1",
                CreatedAt = DateTimeOffset.UtcNow
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        word.Topics.Should().HaveCount(2);
    }
}

