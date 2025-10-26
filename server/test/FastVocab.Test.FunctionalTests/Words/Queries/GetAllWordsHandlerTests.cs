using AutoMapper;
using FastVocab.Application.Features.Words.Queries.GetAllWords;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Words;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace FastVocab.Test.FunctionalTests.Words.Queries;

public class GetAllWordsHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllWordsHandler _handler;

    public GetAllWordsHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllWordsHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllNonDeletedWords()
    {
        // Arrange
        var query = new GetAllWordsQuery();

        var words = new List<Word>
        {
            new() { Id = 1, Text = "hello", Meaning = "xin chào", Type = "Noun", Level = "A1", IsDeleted = false },
            new() { Id = 2, Text = "goodbye", Meaning = "tạm biệt", Type = "Noun", Level = "A1", IsDeleted = false },
            new() { Id = 3, Text = "thanks", Meaning = "cảm ơn", Type = "Noun", Level = "A1", IsDeleted = false }
        };

        var wordDtos = new List<WordDto>
        {
            new() { Id = 1, Text = "hello", Meaning = "xin chào", Type = "Noun", Level = "A1", CreatedAt = DateTimeOffset.UtcNow },
            new() { Id = 2, Text = "goodbye", Meaning = "tạm biệt", Type = "Noun", Level = "A1", CreatedAt = DateTimeOffset.UtcNow },
            new() { Id = 3, Text = "thanks", Meaning = "cảm ơn", Type = "Noun", Level = "A1", CreatedAt = DateTimeOffset.UtcNow }
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
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_WithNoWords_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetAllWordsQuery();

        _unitOfWorkMock.Setup(x => x.Words.GetAllAsync(
            It.IsAny<Expression<Func<Word, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Word>());

        _mapperMock.Setup(x => x.Map<IEnumerable<WordDto>>(It.IsAny<List<Word>>()))
            .Returns(new List<WordDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}

