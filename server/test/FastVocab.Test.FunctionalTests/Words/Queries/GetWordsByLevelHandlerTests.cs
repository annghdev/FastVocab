using AutoMapper;
using FastVocab.Application.Features.Words.Queries.GetWordsByLevel;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Words;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace FastVocab.Test.FunctionalTests.Words.Queries;

public class GetWordsByLevelHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetWordsByLevelHandler _handler;

    public GetWordsByLevelHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetWordsByLevelHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Theory]
    [InlineData("A1")]
    [InlineData("A2")]
    [InlineData("B1")]
    [InlineData("B2")]
    [InlineData("C1")]
    [InlineData("C2")]
    public async Task Handle_WithValidLevel_ShouldReturnWords(string level)
    {
        // Arrange
        var query = new GetWordsByLevelQuery(level);

        var words = new List<Word>
        {
            new() { Id = 1, Text = "test1", Meaning = "test1", Type = "Noun", Level = level, IsDeleted = false },
            new() { Id = 2, Text = "test2", Meaning = "test2", Type = "Verb", Level = level, IsDeleted = false }
        };

        var wordDtos = new List<WordDto>
        {
            new() { Id = 1, Text = "test1", Meaning = "test1", Type = "Noun", Level = level, CreatedAt = DateTimeOffset.UtcNow },
            new() { Id = 2, Text = "test2", Meaning = "test2", Type = "Verb", Level = level, CreatedAt = DateTimeOffset.UtcNow }
        };

        _unitOfWorkMock.Setup(x => x.Words.GetAllAsync(
            It.IsAny<Expression<Func<Word, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(words);

        _mapperMock.Setup(x => x.Map<IEnumerable<WordDto>>(words))
            .Returns(wordDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Should().AllSatisfy(w => w.Level.Should().Be(level));
    }

    [Fact]
    public async Task Handle_WithNoWordsForLevel_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetWordsByLevelQuery("C2");

        _unitOfWorkMock.Setup(x => x.Words.GetAllAsync(
            It.IsAny<Expression<Func<Word, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Word>());

        _mapperMock.Setup(x => x.Map<IEnumerable<WordDto>>(It.IsAny<List<Word>>()))
            .Returns(new List<WordDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }
}

