using FastVocab.API.Controllers;
using FastVocab.Application.Features.Topics.Commands.CreateTopic;
using FastVocab.Application.Features.Topics.Commands.DeleteTopic;
using FastVocab.Application.Features.Topics.Commands.ToggleTopicVisibility;
using FastVocab.Application.Features.Topics.Commands.UpdateTopic;
using FastVocab.Application.Features.Topics.Queries.GetAllTopics;
using FastVocab.Application.Features.Topics.Queries.GetTopicById;
using FastVocab.Application.Features.Topics.Queries.GetVisibleTopics;
using FastVocab.Shared.DTOs.Topics;
using FastVocab.Shared.Utils;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FastVocab.API.Test.Controllers;

public class TopicsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly TopicsController _controller;

    public TopicsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new TopicsController(_mediatorMock.Object);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenTopicsExist()
    {
        // Arrange
        var topics = new List<TopicDto>
        {
            new() { Id = 1, Name = "Technology", VnText = "Công nghệ", IsHiding = false, CreatedAt = DateTimeOffset.UtcNow },
            new() { Id = 2, Name = "Education", VnText = "Giáo dục", IsHiding = false, CreatedAt = DateTimeOffset.UtcNow }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllTopicsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(topics);

        // Act
        var response = await _controller.GetAll(CancellationToken.None);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        var okResult = response as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(topics);
        _mediatorMock.Verify(m => m.Send(It.IsAny<GetAllTopicsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenNoTopicsExist()
    {
        // Arrange
        var result = new List<TopicDto>();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllTopicsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.GetAll(CancellationToken.None);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        var okResult = response as OkObjectResult;
        var topics = okResult!.Value as IEnumerable<TopicDto>;
        topics.Should().BeEmpty();
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenTopicExists()
    {
        // Arrange
        var topicDto = new TopicDto
        {
            Id = 1,
            Name = "Technology",
            VnText = "Công nghệ",
            ImageUrl = "https://example.com/tech.jpg",
            IsHiding = false,
            CreatedAt = DateTimeOffset.UtcNow
        };
        var result = Result<TopicDto>.Success(topicDto);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetTopicByIdQuery>(q => q.TopicId == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.GetById(1, CancellationToken.None);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        var okResult = response as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(topicDto);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenTopicDoesNotExist()
    {
        // Arrange
        var result = Result<TopicDto>.Failure(Error.NotFound);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetTopicByIdQuery>(q => q.TopicId == 999), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.GetById(999, CancellationToken.None);

        // Assert
        response.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region GetVisible Tests

    [Fact]
    public async Task GetVisible_ShouldReturnOk_WhenVisibleTopicsExist()
    {
        // Arrange
        var topics = new List<TopicDto>
        {
            new() { Id = 1, Name = "Technology", VnText = "Công nghệ", IsHiding = false, CreatedAt = DateTimeOffset.UtcNow }
        };
        var result = Result<IEnumerable<TopicDto>>.Success(topics);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetVisibleTopicsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.GetVisible(CancellationToken.None);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        var okResult = response as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(topics);
    }

    [Fact]
    public async Task GetVisible_ShouldReturnOk_WhenNoVisibleTopics()
    {
        // Arrange
        var result = Result<IEnumerable<TopicDto>>.Success(new List<TopicDto>());

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetVisibleTopicsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.GetVisible(CancellationToken.None);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        var okResult = response as OkObjectResult;
        var topics = okResult!.Value as IEnumerable<TopicDto>;
        topics.Should().BeEmpty();
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction_WhenSuccessful()
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = "Science",
            VnText = "Khoa học",
            ImageUrl = "https://example.com/science.jpg"
        };

        var topicDto = new TopicDto
        {
            Id = 1,
            Name = request.Name,
            VnText = request.VnText,
            ImageUrl = request.ImageUrl,
            IsHiding = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var result = Result<TopicDto>.Success(topicDto);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateTopicCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.Create(request, CancellationToken.None);

        // Assert
        response.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = response as CreatedAtActionResult;
        createdResult!.ActionName.Should().Be(nameof(TopicsController.GetById));
        createdResult.RouteValues!["id"].Should().Be(1);
        createdResult.Value.Should().BeEquivalentTo(topicDto);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = "",  // Invalid
            VnText = "Test"
        };

        var errors = new List<string> { "Name is required" };
        var result = Result<TopicDto>.Failure(Error.InvalidInput);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateTopicCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.Create(request, CancellationToken.None);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenDuplicateName()
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = "Existing Topic",
            VnText = "Chủ đề đã tồn tại"
        };

        var result = Result<TopicDto>.Failure(Error.Duplicate);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateTopicCommand>(), It.IsAny<CancellationToken>()))
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
        var request = new UpdateTopicRequest
        {
            Id = 1,
            Name = "Updated Technology",
            VnText = "Công nghệ cập nhật",
            IsHiding = false
        };

        var topicDto = new TopicDto
        {
            Id = 1,
            Name = request.Name,
            VnText = request.VnText,
            IsHiding = request.IsHiding,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var result = Result<TopicDto>.Success(topicDto);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateTopicCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.Update(1, request, CancellationToken.None);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        var okResult = response as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(topicDto);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenIdMismatch()
    {
        // Arrange
        var request = new UpdateTopicRequest
        {
            Id = 2,
            Name = "Test",
            VnText = "Test"
        };

        // Act
        var response = await _controller.Update(1, request, CancellationToken.None);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = response as BadRequestObjectResult;
        badRequestResult!.Value.Should().Be("ID in URL does not match ID in request body.");
        
        _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateTopicCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenTopicDoesNotExist()
    {
        // Arrange
        var request = new UpdateTopicRequest
        {
            Id = 999,
            Name = "Test",
            VnText = "Test"
        };

        var result = Result<TopicDto>.Failure(Error.NotFound);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateTopicCommand>(), It.IsAny<CancellationToken>()))
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
            .Setup(m => m.Send(It.Is<DeleteTopicCommand>(c => c.TopicId == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.Delete(1, CancellationToken.None);

        // Assert
        response.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenTopicDoesNotExist()
    {
        // Arrange
        var result = Result.Failure(Error.NotFound);

        _mediatorMock
            .Setup(m => m.Send(It.Is<DeleteTopicCommand>(c => c.TopicId == 999), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.Delete(999, CancellationToken.None);

        // Assert
        response.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region ToggleVisibility Tests

    [Fact]
    public async Task ToggleVisibility_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var topicDto = new TopicDto
        {
            Id = 1,
            Name = "Technology",
            VnText = "Công nghệ",
            IsHiding = true,  // Toggled
            CreatedAt = DateTimeOffset.UtcNow
        };

        var result = Result<TopicDto>.Success(topicDto);

        _mediatorMock
            .Setup(m => m.Send(It.Is<ToggleTopicVisibilityCommand>(c => c.TopicId == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.ToggleVisibility(1, CancellationToken.None);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        var okResult = response as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(topicDto);
    }

    [Fact]
    public async Task ToggleVisibility_ShouldReturnNotFound_WhenTopicDoesNotExist()
    {
        // Arrange
        var result = Result<TopicDto>.Failure(Error.NotFound);

        _mediatorMock
            .Setup(m => m.Send(It.Is<ToggleTopicVisibilityCommand>(c => c.TopicId == 999), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await _controller.ToggleVisibility(999, CancellationToken.None);

        // Assert
        response.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion
}

