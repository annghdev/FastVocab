using AutoMapper;
using FastVocab.Application.Features.Topics.Queries.GetTopicById;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Topics;
using FluentAssertions;
using Moq;

namespace FastVocab.Test.FunctionalTests.Topics.Queries;

public class GetTopicByIdHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTopicByIdHandler _handler;

    public GetTopicByIdHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetTopicByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ShouldReturnTopic()
    {
        // Arrange
        var topicId = 1;
        var query = new GetTopicByIdQuery(topicId);

        var topic = new Topic
        {
            Id = topicId,
            Name = "Business English",
            VnText = "Tiếng Anh Thương Mại",
            IsHiding = false,
            IsDeleted = false
        };

        var topicDto = new TopicDto
        {
            Id = topicId,
            Name = topic.Name,
            VnText = topic.VnText,
            IsHiding = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(topicId))
            .ReturnsAsync(topic);

        _mapperMock.Setup(x => x.Map<TopicDto>(topic))
            .Returns(topicDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(topicId);
        result.Data.Name.Should().Be(topic.Name);
        result.Data.VnText.Should().Be(topic.VnText);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ShouldReturnFailure()
    {
        // Arrange
        var topicId = 999;
        var query = new GetTopicByIdQuery(topicId);

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
        var query = new GetTopicByIdQuery(topicId);

        var topic = new Topic
        {
            Id = topicId,
            Name = "Deleted Topic",
            VnText = "Đã Xóa",
            IsDeleted = true
        };

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(topicId))
            .ReturnsAsync(topic);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors?.FirstOrDefault()?.Title.Should().Contain("deleted");
    }

    [Fact]
    public async Task Handle_WithHiddenTopic_ShouldStillReturnTopic()
    {
        // Arrange
        var topicId = 1;
        var query = new GetTopicByIdQuery(topicId);

        var topic = new Topic
        {
            Id = topicId,
            Name = "Hidden Topic",
            VnText = "Chủ đề ẩn",
            IsHiding = true,
            IsDeleted = false
        };

        var topicDto = new TopicDto
        {
            Id = topicId,
            Name = topic.Name,
            VnText = topic.VnText,
            IsHiding = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _unitOfWorkMock.Setup(x => x.Topics.FindAsync(topicId))
            .ReturnsAsync(topic);

        _mapperMock.Setup(x => x.Map<TopicDto>(topic))
            .Returns(topicDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.IsHiding.Should().BeTrue();
    }
}

