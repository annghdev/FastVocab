using AutoMapper;
using FastVocab.Application.Features.Topics.Queries.GetAllTopics;
using FastVocab.Domain.Entities.CoreEntities;
using FastVocab.Domain.Repositories;
using FastVocab.Shared.DTOs.Topics;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace FastVocab.Application.Test.Features.Topics.Queries;

public class GetAllTopicsHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllTopicsHandler _handler;

    public GetAllTopicsHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllTopicsHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllNonDeletedTopics()
    {
        // Arrange
        var query = new GetAllTopicsQuery();

        var topics = new List<Topic>
        {
            new() { Id = 1, Name = "Topic 1", VnText = "Chủ đề 1", IsHiding = false, IsDeleted = false },
            new() { Id = 2, Name = "Topic 2", VnText = "Chủ đề 2", IsHiding = true, IsDeleted = false },
            new() { Id = 3, Name = "Topic 3", VnText = "Chủ đề 3", IsHiding = false, IsDeleted = false }
        };

        var topicDtos = new List<TopicDto>
        {
            new() { Id = 1, Name = "Topic 1", VnText = "Chủ đề 1", IsHiding = false, CreatedAt = DateTimeOffset.UtcNow },
            new() { Id = 2, Name = "Topic 2", VnText = "Chủ đề 2", IsHiding = true, CreatedAt = DateTimeOffset.UtcNow },
            new() { Id = 3, Name = "Topic 3", VnText = "Chủ đề 3", IsHiding = false, CreatedAt = DateTimeOffset.UtcNow }
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
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(dto => dto.Id == 1);
        result.Should().Contain(dto => dto.Id == 2 && dto.IsHiding == true);
        result.Should().Contain(dto => dto.Id == 3);
    }

    [Fact]
    public async Task Handle_WithNoTopics_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new GetAllTopicsQuery();

        _unitOfWorkMock.Setup(x => x.Topics.GetAllAsync(
            It.IsAny<Expression<Func<Topic, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Topic>());

        _mapperMock.Setup(x => x.Map<IEnumerable<TopicDto>>(It.IsAny<List<Topic>>()))
            .Returns(new List<TopicDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldIncludeHiddenTopics()
    {
        // Arrange
        var query = new GetAllTopicsQuery();

        var topics = new List<Topic>
        {
            new() { Id = 1, Name = "Visible Topic", VnText = "Hiển thị", IsHiding = false, IsDeleted = false },
            new() { Id = 2, Name = "Hidden Topic", VnText = "Ẩn", IsHiding = true, IsDeleted = false }
        };

        var topicDtos = new List<TopicDto>
        {
            new() { Id = 1, Name = "Visible Topic", VnText = "Hiển thị", IsHiding = false, CreatedAt = DateTimeOffset.UtcNow },
            new() { Id = 2, Name = "Hidden Topic", VnText = "Ẩn", IsHiding = true, CreatedAt = DateTimeOffset.UtcNow }
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
        result.Should().HaveCount(2);
        result.Should().Contain(dto => dto.IsHiding == true);
    }
}

