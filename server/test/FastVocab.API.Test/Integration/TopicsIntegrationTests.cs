using System.Net;
using System.Net.Http.Json;
using FastVocab.API.Test.Setups;
using FastVocab.Shared.DTOs.Topics;
using FluentAssertions;

namespace FastVocab.API.Test.Integration;

/// <summary>
/// Integration tests for Topics API endpoints
/// Tests the full request/response pipeline including validation, database, and middleware
/// </summary>
public class TopicsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public TopicsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.InitializeDatabase();
        _client = factory.CreateClient();
    }

    #region Create Topic Tests

    [Fact]
    public async Task CreateTopic_WithValidData_ShouldReturnCreatedTopic()
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = "Technology",
            VnText = "Công nghệ",
            ImageUrl = "https://example.com/tech.jpg"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/topics", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var topic = await response.Content.ReadFromJsonAsync<TopicDto>();
        topic.Should().NotBeNull();
        topic!.Name.Should().Be("Technology");
        topic.VnText.Should().Be("Công nghệ");
        topic.IsHiding.Should().BeFalse();
    }

    [Fact]
    public async Task CreateTopic_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = "",
            VnText = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/topics", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadAsStringAsync();
        error.Should().Contain("Name");
    }

    [Fact]
    public async Task CreateTopic_WithNameTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateTopicRequest
        {
            Name = new string('a', 201),
            VnText = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/topics", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadAsStringAsync();
        error.Should().Contain("Name");
    }

    [Fact]
    public async Task CreateTopic_WithDuplicateName_ShouldReturnBadRequest()
    {
        // Arrange
        var request1 = new CreateTopicRequest
        {
            Name = "Duplicate",
            VnText = "Test 1"
        };
        var request2 = new CreateTopicRequest
        {
            Name = "Duplicate",
            VnText = "Test 2"
        };

        // Act
        await _client.PostAsJsonAsync("/api/topics", request1);
        var response = await _client.PostAsJsonAsync("/api/topics", request2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Get Topics Tests

    [Fact]
    public async Task GetAllTopics_ShouldReturnAllTopics()
    {
        // Arrange - Create some topics
        await CreateTopicAsync("Topic1", "Chủ đề 1");
        await CreateTopicAsync("Topic2", "Chủ đề 2");

        // Act
        var response = await _client.GetAsync("/api/topics");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var topics = await response.Content.ReadFromJsonAsync<List<TopicDto>>();
        topics.Should().NotBeNull();
        topics!.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetVisibleTopics_ShouldReturnOnlyVisibleTopics()
    {
        // Arrange - Create topics and hide one
        var topicId = await CreateTopicAsync("VisibleTest", "Test hiển thị");
        await _client.PatchAsync($"/api/topics/{topicId}/visibility", null);

        // Act
        var response = await _client.GetAsync("/api/topics/visible");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var topics = await response.Content.ReadFromJsonAsync<List<TopicDto>>();
        topics.Should().NotBeNull();
        topics!.Should().NotContain(t => t.Name == "VisibleTest");
    }

    [Fact]
    public async Task GetTopicById_WithValidId_ShouldReturnTopic()
    {
        // Arrange
        var topicId = await CreateTopicAsync("GetByIdTest", "Test lấy theo ID");

        // Act
        var response = await _client.GetAsync($"/api/topics/{topicId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var topic = await response.Content.ReadFromJsonAsync<TopicDto>();
        topic.Should().NotBeNull();
        topic!.Name.Should().Be("GetByIdTest");
    }

    [Fact]
    public async Task GetTopicById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/topics/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Update Topic Tests

    [Fact]
    public async Task UpdateTopic_WithValidData_ShouldReturnUpdatedTopic()
    {
        // Arrange
        var topicId = await CreateTopicAsync("UpdateTest", "Test cập nhật");
        var updateRequest = new UpdateTopicRequest
        {
            Id = topicId,
            Name = "UpdatedName",
            VnText = "Tên đã cập nhật",
            IsHiding = false
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/topics/{topicId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var topic = await response.Content.ReadFromJsonAsync<TopicDto>();
        topic.Should().NotBeNull();
        topic!.Name.Should().Be("UpdatedName");
        topic.VnText.Should().Be("Tên đã cập nhật");
    }

    [Fact]
    public async Task UpdateTopic_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        var updateRequest = new UpdateTopicRequest
        {
            Id = 99999,
            Name = "Test",
            VnText = "Test"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/topics/99999", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTopic_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var topicId = await CreateTopicAsync("ValidName", "Tên hợp lệ");
        var updateRequest = new UpdateTopicRequest
        {
            Id = topicId,
            Name = "",
            VnText = "Test"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/topics/{topicId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Delete Topic Tests

    [Fact]
    public async Task DeleteTopic_WithValidId_ShouldReturnSuccess()
    {
        // Arrange
        var topicId = await CreateTopicAsync("DeleteTest", "Test xóa");

        // Act
        var response = await _client.DeleteAsync($"/api/topics/{topicId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/topics/{topicId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTopic_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/topics/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteTopic_AlreadyDeleted_ShouldReturnNotFound()
    {
        // Arrange
        var topicId = await CreateTopicAsync("DeleteTwiceTest", "Test xóa 2 lần");
        await _client.DeleteAsync($"/api/topics/{topicId}");

        // Act - Try to delete again
        var response = await _client.DeleteAsync($"/api/topics/{topicId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Toggle Visibility Tests

    [Fact]
    public async Task ToggleVisibility_WithValidId_ShouldToggleSuccessfully()
    {
        // Arrange
        var topicId = await CreateTopicAsync("ToggleTest", "Test ẩn/hiện");

        // Act - Toggle to hidden
        var response1 = await _client.PatchAsync($"/api/topics/{topicId}/visibility", null);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var topic1 = await response1.Content.ReadFromJsonAsync<TopicDto>();
        topic1!.IsHiding.Should().BeTrue();

        // Act - Toggle back to visible
        var response2 = await _client.PatchAsync($"/api/topics/{topicId}/visibility", null);

        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var topic2 = await response2.Content.ReadFromJsonAsync<TopicDto>();
        topic2!.IsHiding.Should().BeFalse();
    }

    [Fact]
    public async Task ToggleVisibility_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.PatchAsync("/api/topics/99999/visibility", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Helper method to create a topic and return its ID
    /// </summary>
    private async Task<int> CreateTopicAsync(string name, string vnText)
    {
        var request = new CreateTopicRequest
        {
            Name = name,
            VnText = vnText
        };

        var response = await _client.PostAsJsonAsync("/api/topics", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created); // Expect 201 Created

        var topic = await response.Content.ReadFromJsonAsync<TopicDto>();
        return topic!.Id;
    }

    #endregion
}

