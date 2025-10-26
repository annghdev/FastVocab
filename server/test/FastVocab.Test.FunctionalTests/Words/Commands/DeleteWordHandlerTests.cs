using FastVocab.Application.Features.Words.Commands.DeleteWord;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace FastVocab.Test.FunctionalTests.Words.Commands;

public class DeleteWordHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteWordHandler _handler;

    public DeleteWordHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeleteWordHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidWord_ShouldSoftDeleteSuccessfully()
    {
        // Arrange
        var wordId = 1;
        var command = new DeleteWordCommand(wordId);

        var word = new Word
        {
            Id = wordId,
            Text = "test",
            Meaning = "test",
            Type = "Noun",
            Level = "A1",
            IsDeleted = false
        };

        _unitOfWorkMock.Setup(x => x.Words.FindAsync(wordId))
            .ReturnsAsync(word);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        word.IsDeleted.Should().BeTrue();

        _unitOfWorkMock.Verify(x => x.Words.Update(word), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentWord_ShouldReturnFailure()
    {
        // Arrange
        var wordId = 999;
        var command = new DeleteWordCommand(wordId);

        _unitOfWorkMock.Setup(x => x.Words.FindAsync(wordId))
            .ReturnsAsync((Word?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("not found");

        _unitOfWorkMock.Verify(x => x.Words.Update(It.IsAny<Word>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithAlreadyDeletedWord_ShouldReturnFailure()
    {
        // Arrange
        var wordId = 1;
        var command = new DeleteWordCommand(wordId);

        var word = new Word
        {
            Id = wordId,
            Text = "test",
            IsDeleted = true
        };

        _unitOfWorkMock.Setup(x => x.Words.FindAsync(wordId))
            .ReturnsAsync(word);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("already been deleted");

        _unitOfWorkMock.Verify(x => x.Words.Update(It.IsAny<Word>()), Times.Never);
    }
}

