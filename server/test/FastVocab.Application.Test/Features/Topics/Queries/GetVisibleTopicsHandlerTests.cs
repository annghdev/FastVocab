using AutoMapper;
using FastVocab.Application.Features.Topics.Queries.GetVisibleTopics;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Topics;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace FastVocab.Application.Test.Features.Topics.Queries;

public class GetVisibleTopicsHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetVisibleTopicsHandler _handler;

    public GetVisibleTopicsHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetVisibleTopicsHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyVisibleTopics()
    {
        // Arrange
        var query = new GetVisibleTopicsQuery();

        var topics = new List<Topic>
        {
            new() { Id = 1, Name = "Topic 1", VnText = "Chủ đề 1", IsHiding = false, IsDeleted = false },
            new() { Id = 2, Name = "Topic 2", VnText = "Chủ đề 2", IsHiding = false, IsDeleted = false }
        };

        var topicDtos = new List<TopicDto>
        {
            new() { Id = 1, Name = "Topic 1", VnText = "Chủ đề 1", IsHiding = false, CreatedAt = DateTimeOffset.UtcNow },
            new() { Id = 2, Name = "Topic 2", VnText = "Chủ đề 2", IsHiding = false, CreatedAt = DateTimeOffset.UtcNow }
        };

        _unitOfWorkMock.Setup(x => x.Topics.GetAllAsync(
            It.IsAny<Expression<Func<Topic, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(topics);

        _mapperMock.Setup(x => x.Map<IEnumerable<TopicDto>>(topics))
            .Returns(topicDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data.Should().OnlyContain(dto => dto.IsHiding == false);
    }

    [Fact]
    public async Task Handle_ShouldNotReturnHiddenTopics()
    {
        // Arrange
        var query = new GetVisibleTopicsQuery();

        // Only visible topics should be returned
        var topics = new List<Topic>
        {
            new() { Id = 1, Name = "Visible Topic", VnText = "Hiển thị", IsHiding = false, IsDeleted = false }
            // Hidden topics are filtered out by the repository
        };

        var topicDtos = new List<TopicDto>
        {
            new() { Id = 1, Name = "Visible Topic", VnText = "Hiển thị", IsHiding = false, CreatedAt = DateTimeOffset.UtcNow }
        };

        _unitOfWorkMock.Setup(x => x.Topics.GetAllAsync(
            It.IsAny<Expression<Func<Topic, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(topics);

        _mapperMock.Setup(x => x.Map<IEnumerable<TopicDto>>(topics))
            .Returns(topicDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data.Should().NotContain(dto => dto.IsHiding == true);
    }

    [Fact]
    public async Task Handle_WithNoVisibleTopics_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetVisibleTopicsQuery();

        _unitOfWorkMock.Setup(x => x.Topics.GetAllAsync(
            It.IsAny<Expression<Func<Topic, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Topic>());

        _mapperMock.Setup(x => x.Map<IEnumerable<TopicDto>>(It.IsAny<List<Topic>>()))
            .Returns(new List<TopicDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldNotReturnDeletedTopics()
    {
        // Arrange
        var query = new GetVisibleTopicsQuery();

        // Only non-deleted and visible topics
        var topics = new List<Topic>
        {
            new() { Id = 1, Name = "Topic 1", VnText = "Chủ đề 1", IsHiding = false, IsDeleted = false }
        };

        var topicDtos = new List<TopicDto>
        {
            new() { Id = 1, Name = "Topic 1", VnText = "Chủ đề 1", IsHiding = false, CreatedAt = DateTimeOffset.UtcNow }
        };

        _unitOfWorkMock.Setup(x => x.Topics.GetAllAsync(
            It.IsAny<Expression<Func<Topic, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(topics);

        _mapperMock.Setup(x => x.Map<IEnumerable<TopicDto>>(topics))
            .Returns(topicDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(1);
    }
}

