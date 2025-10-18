using AutoMapper;
using FastVocab.Application.Features.Words.Commands.CreateWord;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Entities.JunctionEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Words;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace FastVocab.Application.Test.Features.Words.Commands;

public class CreateWordHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateWordHandler _handler;

    public CreateWordHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateWordHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateWordSuccessfully()
    {
        // Arrange
        var request = new CreateWordRequest
        {
            Text = "perseverance",
            Meaning = "sự kiên trì",
            Type = "Noun",
            Level = "B2"
        };
        var command = new CreateWordCommand(request);

        var word = new Word
        {
            Id = 1,
            Text = request.Text,
            Meaning = request.Meaning,
            Type = request.Type,
            Level = request.Level
        };

        var wordDto = new WordDto
        {
            Id = 1,
            Text = request.Text,
            Meaning = request.Meaning,
            Type = request.Type,
            Level = request.Level,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _unitOfWorkMock.Setup(x => x.Words.FindAsync(It.IsAny<Expression<Func<Word, bool>>>()))
            .ReturnsAsync((Word?)null);

        _mapperMock.Setup(x => x.Map<Word>(request))
            .Returns(word);

        _unitOfWorkMock.Setup(x => x.Words.Add(It.IsAny<Word>()))
            .Returns(word);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapperMock.Setup(x => x.Map<WordDto>(It.IsAny<Word>()))
            .Returns(wordDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Text.Should().Be(request.Text);
        result.Data.Meaning.Should().Be(request.Meaning);

        _unitOfWorkMock.Verify(x => x.Words.Add(It.IsAny<Word>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithDuplicateText_ShouldReturnFailure()
    {
        // Arrange
        var request = new CreateWordRequest
        {
            Text = "perseverance",
            Meaning = "sự kiên trì",
            Type = "Noun",
            Level = "B2"
        };
        var command = new CreateWordCommand(request);

        var existingWord = new Word
        {
            Id = 1,
            Text = request.Text,
            Meaning = "Old meaning"
        };

        _unitOfWorkMock.Setup(x => x.Words.FindAsync(It.IsAny<Expression<Func<Word, bool>>>()))
            .ReturnsAsync(existingWord);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("already exists");

        _unitOfWorkMock.Verify(x => x.Words.Add(It.IsAny<Word>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithTopicIds_ShouldAssociateWithTopics()
    {
        // Arrange
        var request = new CreateWordRequest
        {
            Text = "algorithm",
            Meaning = "thuật toán",
            Type = "Noun",
            Level = "C1",
            TopicIds = new List<int> { 1, 2 }
        };
        var command = new CreateWordCommand(request);

        var word = new Word
        {
            Id = 1,
            Text = request.Text,
            Meaning = request.Meaning,
            Type = request.Type,
            Level = request.Level,
            Topics = new List<WordTopic>()
        };

        var topic1 = new Topic { Id = 1, Name = "Technology", IsDeleted = false };
        var topic2 = new Topic { Id = 2, Name = "Science", IsDeleted = false };

        _unitOfWorkMock.Setup(x => x.Words.FindAsync(It.IsAny<Expression<Func<Word, bool>>>()))
            .ReturnsAsync((Word?)null);

        _mapperMock.Setup(x => x.Map<Word>(request))
            .Returns(word);

        _unitOfWorkMock.Setup(x => x.Words.Add(It.IsAny<Word>()))
            .Returns(word);

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(1))
            .ReturnsAsync(topic1);

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(2))
            .ReturnsAsync(topic2);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapperMock.Setup(x => x.Map<WordDto>(It.IsAny<Word>()))
            .Returns(new WordDto { Id = 1, Text = request.Text, Meaning = request.Meaning, Type = request.Type, Level = request.Level, CreatedAt = DateTimeOffset.UtcNow });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        word.Topics.Should().HaveCount(2);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_WithInvalidTopicId_ShouldReturnFailure()
    {
        // Arrange
        var request = new CreateWordRequest
        {
            Text = "test",
            Meaning = "test",
            Type = "Noun",
            Level = "A1",
            TopicIds = new List<int> { 999 }
        };
        var command = new CreateWordCommand(request);

        var word = new Word
        {
            Id = 1,
            Text = request.Text,
            Topics = new List<WordTopic>()
        };

        _unitOfWorkMock.Setup(x => x.Words.FindAsync(It.IsAny<Expression<Func<Word, bool>>>()))
            .ReturnsAsync((Word?)null);

        _mapperMock.Setup(x => x.Map<Word>(request))
            .Returns(word);

        _unitOfWorkMock.Setup(x => x.Words.Add(It.IsAny<Word>()))
            .Returns(word);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(999))
            .ReturnsAsync((Topic?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("not found");
    }
}

