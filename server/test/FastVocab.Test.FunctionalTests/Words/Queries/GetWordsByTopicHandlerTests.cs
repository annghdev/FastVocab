using AutoMapper;
using FastVocab.Application.Features.Words.Queries.GetWordsByTopic;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Entities.JunctionEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Words;
using FluentAssertions;
using Moq;

namespace FastVocab.Test.FunctionalTests.Words.Queries;

public class GetWordsByTopicHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetWordsByTopicHandler _handler;

    public GetWordsByTopicHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetWordsByTopicHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidTopicId_ShouldReturnWords()
    {
        // Arrange
        var topicId = 1;
        var query = new GetWordsByTopicQuery(topicId);

        var topic = new Topic
        {
            Id = topicId,
            Name = "Test Topic",
            IsDeleted = false,
            Words = new List<WordTopic>
            {
                new() { TopicId = topicId, WordId = 1, Word = new Word { Id = 1, Text = "hello", Meaning = "xin chào", Type = "Noun", Level = "A1", IsDeleted = false } },
                new() { TopicId = topicId, WordId = 2, Word = new Word { Id = 2, Text = "world", Meaning = "thế giới", Type = "Noun", Level = "A1", IsDeleted = false } }
            }
        };

        var wordDtos = new List<WordDto>
        {
            new() { Id = 1, Text = "hello", Meaning = "xin chào", Type = "Noun", Level = "A1", CreatedAt = DateTimeOffset.UtcNow },
            new() { Id = 2, Text = "world", Meaning = "thế giới", Type = "Noun", Level = "A1", CreatedAt = DateTimeOffset.UtcNow }
        };

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(topicId))
            .ReturnsAsync(topic);

        _unitOfWorkMock.Setup(x => x.Words.GetByTopic(topicId))
            .ReturnsAsync(topic.Words.Select(wt => wt.Word!).ToList());

        _mapperMock.Setup(x => x.Map<IEnumerable<WordDto>>(It.IsAny<IEnumerable<Word>>()))
            .Returns(wordDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithNonExistentTopic_ShouldReturnFailure()
    {
        // Arrange
        var topicId = 999;
        var query = new GetWordsByTopicQuery(topicId);

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(topicId))
            .ReturnsAsync((Topic?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_WithDeletedTopic_ShouldReturnFailure()
    {
        // Arrange
        var topicId = 1;
        var query = new GetWordsByTopicQuery(topicId);

        var topic = new Topic
        {
            Id = topicId,
            Name = "Deleted Topic",
            IsDeleted = true
        };

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(topicId))
            .ReturnsAsync(topic);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_WithTopicWithoutWords_ShouldReturnEmptyList()
    {
        // Arrange
        var topicId = 1;
        var query = new GetWordsByTopicQuery(topicId);

        var topic = new Topic
        {
            Id = topicId,
            Name = "Empty Topic",
            IsDeleted = false,
            Words = new List<WordTopic>()
        };

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(topicId))
            .ReturnsAsync(topic);

        _unitOfWorkMock.Setup(x => x.Words.GetByTopic(topicId))
            .ReturnsAsync(new List<Word>());

        _mapperMock.Setup(x => x.Map<IEnumerable<WordDto>>(It.IsAny<IEnumerable<Word>>()))
            .Returns(new List<WordDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }
}

