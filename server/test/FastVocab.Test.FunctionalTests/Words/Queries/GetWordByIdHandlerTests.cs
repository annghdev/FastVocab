using AutoMapper;
using FastVocab.Application.Features.Words.Queries.GetWordById;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Words;
using FluentAssertions;
using Moq;

namespace FastVocab.Test.FunctionalTests.Words.Queries;

public class GetWordByIdHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetWordByIdHandler _handler;

    public GetWordByIdHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetWordByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ShouldReturnWord()
    {
        // Arrange
        var wordId = 1;
        var query = new GetWordByIdQuery(wordId);

        var word = new Word
        {
            Id = wordId,
            Text = "perseverance",
            Meaning = "sự kiên trì",
            Type = "Noun",
            Level = "B2",
            IsDeleted = false
        };

        var wordDto = new WordDto
        {
            Id = wordId,
            Text = word.Text,
            Meaning = word.Meaning,
            Type = word.Type,
            Level = word.Level,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _unitOfWorkMock.Setup(x => x.Words.FindAsync(wordId))
            .ReturnsAsync(word);

        _mapperMock.Setup(x => x.Map<WordDto>(word))
            .Returns(wordDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(wordId);
        result.Data.Text.Should().Be(word.Text);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ShouldReturnFailure()
    {
        // Arrange
        var wordId = 999;
        var query = new GetWordByIdQuery(wordId);

        _unitOfWorkMock.Setup(x => x.Words.FindAsync(wordId))
            .ReturnsAsync((Word?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("not found");
    }

    [Fact]
    public async Task Handle_WithDeletedWord_ShouldReturnFailure()
    {
        // Arrange
        var wordId = 1;
        var query = new GetWordByIdQuery(wordId);

        var word = new Word
        {
            Id = wordId,
            Text = "deleted",
            IsDeleted = true
        };

        _unitOfWorkMock.Setup(x => x.Words.FindAsync(wordId))
            .ReturnsAsync(word);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("deleted");
    }
}

