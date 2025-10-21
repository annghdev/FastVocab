using FastVocab.API.Controllers;
using FastVocab.Application.Features.Words.Commands.AddWordToTopic;
using FastVocab.Application.Features.Words.Commands.CreateWord;
using FastVocab.Application.Features.Words.Commands.DeleteWord;
using FastVocab.Application.Features.Words.Commands.RemoveWordFromTopic;
using FastVocab.Application.Features.Words.Commands.UpdateWord;
using FastVocab.Application.Features.Words.Queries.GetAllWords;
using FastVocab.Application.Features.Words.Queries.GetWordById;
using FastVocab.Application.Features.Words.Queries.GetWordsByLevel;
using FastVocab.Application.Features.Words.Queries.GetWordsByTopic;
using FastVocab.Shared.DTOs.Words;
using FastVocab.Shared.Utils;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FastVocab.API.Test.Controllers;

public class WordsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly WordsController _controller;

    public WordsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new WordsController(_mediatorMock.Object);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenWordsExist()
    {
        // Arrange
        var words = new List<WordDto>
        {
            new() { Id = 1, Text = "hello", Meaning = "xin chào", Type = "Noun", Level = "A1", CreatedAt = DateTimeOffset.UtcNow },
            new() { Id = 2, Text = "world", Meaning = "thế giới", Type = "Noun", Level = "A1", CreatedAt = DateTimeOffset.UtcNow }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllWordsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(words);

        // Act
        var response = await _controller.GetAll(CancellationToken.None);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        var okResult = response as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(words);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllWordsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenNoWordsExist()
    {
        // Arrange
        var result = new List<WordDto>();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllWordsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.GetAll(CancellationToken.None);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        var okResult = response as OkObjectResult;
        var words = okResult!.Value as IEnumerable<WordDto>;
        words.Should().BeEmpty();
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenWordExists()
    {
        // Arrange
        var wordDto = new WordDto
        {
            Id = 1,
            Text = "perseverance",
            Meaning = "sự kiên trì",
            Type = "Noun",
            Level = "B2",
            CreatedAt = DateTimeOffset.UtcNow
        };
        var result = Result<WordDto>.Success(wordDto);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetWordByIdQuery>(q => q.WordId == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.GetById(1, CancellationToken.None);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        var okResult = response as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(wordDto);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenWordDoesNotExist()
    {
        // Arrange
        var result = Result<WordDto>.Failure(Error.NotFound);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetWordByIdQuery>(q => q.WordId == 999), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.GetById(999, CancellationToken.None);

        // Assert
        response.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region GetByTopic Tests

    [Fact]
    public async Task GetByTopic_ShouldReturnOk_WhenTopicHasWords()
    {
        // Arrange
        var words = new List<WordDto>
        {
            new() { Id = 1, Text = "computer", Meaning = "máy tính", Type = "Noun", Level = "A2", CreatedAt = DateTimeOffset.UtcNow }
        };
        var result = Result<IEnumerable<WordDto>>.Success(words);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetWordsByTopicQuery>(q => q.TopicId == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.GetByTopic(1, CancellationToken.None);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        var okResult = response as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(words);
    }

    [Fact]
    public async Task GetByTopic_ShouldReturnNotFound_WhenTopicDoesNotExist()
    {
        // Arrange
        var result = Result<IEnumerable<WordDto>>.Failure(Error.NotFound);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetWordsByTopicQuery>(q => q.TopicId == 999), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.GetByTopic(999, CancellationToken.None);

        // Assert
        response.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region GetByLevel Tests

    [Fact]
    public async Task GetByLevel_ShouldReturnOk_WhenWordsExist()
    {
        // Arrange
        var words = new List<WordDto>
        {
            new() { Id = 1, Text = "hello", Meaning = "xin chào", Type = "Noun", Level = "A1", CreatedAt = DateTimeOffset.UtcNow }
        };
        var result = Result<IEnumerable<WordDto>>.Success(words);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetWordsByLevelQuery>(q => q.Level == "A1"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.GetByLevel("A1", CancellationToken.None);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        var okResult = response as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(words);
    }

    [Fact]
    public async Task GetByLevel_ShouldReturnOk_WhenNoWordsForLevel()
    {
        // Arrange
        var result = Result<IEnumerable<WordDto>>.Success(new List<WordDto>());

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetWordsByLevelQuery>(q => q.Level == "C2"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.GetByLevel("C2", CancellationToken.None);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        var okResult = response as OkObjectResult;
        var words = okResult!.Value as IEnumerable<WordDto>;
        words.Should().BeEmpty();
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction_WhenSuccessful()
    {
        // Arrange
        var request = new CreateWordRequest
        {
            Text = "algorithm",
            Meaning = "thuật toán",
            Type = "Noun",
            Level = "C1",
            Definition = "A process or set of rules",
            Example1 = "The algorithm processes data."
        };

        var wordDto = new WordDto
        {
            Id = 1,
            Text = request.Text,
            Meaning = request.Meaning,
            Type = request.Type,
            Level = request.Level,
            Definition = request.Definition,
            Example1 = request.Example1,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var result = Result<WordDto>.Success(wordDto);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateWordCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.Create(request, CancellationToken.None);

        // Assert
        response.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = response as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(nameof(WordsController.GetById));
        createdResult.RouteValues!["id"].Should().Be(1);
        createdResult.Value.Should().BeEquivalentTo(wordDto);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = new CreateWordRequest
        {
            Text = "",  // Invalid
            Meaning = "test",
            Type = "Noun",
            Level = "A1"
        };

        var result = Result<WordDto>.Failure(Error.InvalidInput);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateWordCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.Create(request, CancellationToken.None);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenDuplicateText()
    {
        // Arrange
        var request = new CreateWordRequest
        {
            Text = "existing",
            Meaning = "đã tồn tại",
            Type = "Noun",
            Level = "A1"
        };

        var result = Result<WordDto>.Failure(Error.Duplicate);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateWordCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.Create(request, CancellationToken.None);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task Update_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = 1,
            Text = "updated",
            Meaning = "đã cập nhật",
            Type = "Verb",
            Level = "B1"
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

        var result = Result<WordDto>.Success(wordDto);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateWordCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.Update(1, request, CancellationToken.None);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        var okResult = response as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(wordDto);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
    {
        // Arrange
        var request = new UpdateWordRequest
        {
            Id = 2,
            Text = "test",
            Meaning = "test",
            Type = "Noun",
            Level = "A1"
        };

        // Act
        var response = await _controller.Update(1, request, CancellationToken.None);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = response as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("ID in URL does not match ID in request body.");

        _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateWordCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenWordDoesNotExist()
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

        var result = Result<WordDto>.Failure(Error.NotFound);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateWordCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.Update(999, request, CancellationToken.None);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenSuccessful()
    {
        // Arrange
        var result = Result.Success();

        _mediatorMock
            .Setup(m => m.Send(It.Is<DeleteWordCommand>(c => c.WordId == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.Delete(1, CancellationToken.None);

        // Assert
        response.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenWordDoesNotExist()
    {
        // Arrange
        var result = Result.Failure(Error.NotFound);

        _mediatorMock
            .Setup(m => m.Send(It.Is<DeleteWordCommand>(c => c.WordId == 999), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.Delete(999, CancellationToken.None);

        // Assert
        response.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region AddToTopic Tests

    [Fact]
    public async Task AddToTopic_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var result = Result.Success();

        _mediatorMock
            .Setup(m => m.Send(It.Is<AddWordToTopicCommand>(c => c.WordId == 1 && c.TopicId == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.AddToTopic(1, 1, CancellationToken.None);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task AddToTopic_ShouldReturnBadRequest_WhenWordOrTopicNotFound()
    {
        // Arrange
        var result = Result.Failure(Error.NotFound);

        _mediatorMock
            .Setup(m => m.Send(It.Is<AddWordToTopicCommand>(c => c.WordId == 999 && c.TopicId == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.AddToTopic(999, 1, CancellationToken.None);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task AddToTopic_ShouldReturnBadRequest_WhenAlreadyAssociated()
    {
        // Arrange
        var result = Result.Failure(Error.Conflict);

        _mediatorMock
            .Setup(m => m.Send(It.Is<AddWordToTopicCommand>(c => c.WordId == 1 && c.TopicId == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.AddToTopic(1, 1, CancellationToken.None);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region RemoveFromTopic Tests

    [Fact]
    public async Task RemoveFromTopic_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var result = Result.Success();

        _mediatorMock
            .Setup(m => m.Send(It.Is<RemoveWordFromTopicCommand>(c => c.WordId == 1 && c.TopicId == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.RemoveFromTopic(1, 1, CancellationToken.None);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task RemoveFromTopic_ShouldReturnBadRequest_WhenWordOrTopicNotFound()
    {
        // Arrange
        var result = Result.Failure(Error.NotFound);

        _mediatorMock
            .Setup(m => m.Send(It.Is<RemoveWordFromTopicCommand>(c => c.WordId == 999 && c.TopicId == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.RemoveFromTopic(999, 1, CancellationToken.None);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task RemoveFromTopic_ShouldReturnBadRequest_WhenNotAssociated()
    {
        // Arrange
        var result = Result.Failure(Error.Conflict);

        _mediatorMock
            .Setup(m => m.Send(It.Is<RemoveWordFromTopicCommand>(c => c.WordId == 1 && c.TopicId == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.RemoveFromTopic(1, 1, CancellationToken.None);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion
}
