using AutoMapper;
using FastVocab.Application.Features.Topics.Commands.UpdateTopic;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Topics;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace FastVocab.Application.Test.Features.Topics.Commands;

public class UpdateTopicHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateTopicHandler _handler;

    public UpdateTopicHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateTopicHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldUpdateTopicSuccessfully()
    {
        // Arrange
        var request = new UpdateTopicRequest
        {
            Id = 1,
            Name = "Business English Updated",
            VnText = "Tiếng Anh Thương Mại Cập Nhật",
            ImageUrl = "https://example.com/new-image.jpg",
            IsHiding = true
        };
        var command = new UpdateTopicCommand(request);

        var existingTopic = new Topic
        {
            Id = 1,
            Name = "Old Name",
            VnText = "Old VnText",
            IsDeleted = false
        };

        var topicDto = new TopicDto
        {
            Id = 1,
            Name = request.Name,
            VnText = request.VnText,
            ImageUrl = request.ImageUrl,
            IsHiding = request.IsHiding,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(request.Id))
            .ReturnsAsync(existingTopic);

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(It.IsAny<Expression<Func<Topic, bool>>>()))
            .ReturnsAsync((Topic?)null); // No duplicate name

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mapperMock.Setup(x => x.Map<TopicDto>(It.IsAny<Topic>()))
            .Returns(topicDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(request.Name);
        result.Data.VnText.Should().Be(request.VnText);
        result.Data.IsHiding.Should().BeTrue();

        existingTopic.Name.Should().Be(request.Name);
        existingTopic.VnText.Should().Be(request.VnText);
        existingTopic.IsHiding.Should().BeTrue();

        _unitOfWorkMock.Verify(x => x.Topics.Update(existingTopic), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentTopic_ShouldReturnFailure()
    {
        // Arrange
        var request = new UpdateTopicRequest
        {
            Id = 999,
            Name = "Non-existent",
            VnText = "Không tồn tại",
            IsHiding = false
        };
        var command = new UpdateTopicCommand(request);

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(request.Id))
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
    public async Task Handle_WithDeletedTopic_ShouldReturnFailure()
    {
        // Arrange
        var request = new UpdateTopicRequest
        {
            Id = 1,
            Name = "Deleted Topic",
            VnText = "Đã Xóa",
            IsHiding = false
        };
        var command = new UpdateTopicCommand(request);

        var deletedTopic = new Topic
        {
            Id = 1,
            Name = "Old Name",
            VnText = "Old VnText",
            IsDeleted = true
        };

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(request.Id))
            .ReturnsAsync(deletedTopic);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("deleted");

        _unitOfWorkMock.Verify(x => x.Topics.Update(It.IsAny<Topic>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithDuplicateName_ShouldReturnFailure()
    {
        // Arrange
        var request = new UpdateTopicRequest
        {
            Id = 1,
            Name = "Duplicate Name",
            VnText = "Tên Trùng",
            IsHiding = false
        };
        var command = new UpdateTopicCommand(request);

        var existingTopic = new Topic
        {
            Id = 1,
            Name = "Old Name",
            VnText = "Old VnText",
            IsDeleted = false
        };

        var anotherTopic = new Topic
        {
            Id = 2,
            Name = "Duplicate Name",
            VnText = "Another VnText",
            IsDeleted = false
        };

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(request.Id))
            .ReturnsAsync(existingTopic);

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(It.IsAny<Expression<Func<Topic, bool>>>()))
            .ReturnsAsync(anotherTopic);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("already exists");

        _unitOfWorkMock.Verify(x => x.Topics.Update(It.IsAny<Topic>()), Times.Never);
    }
}

