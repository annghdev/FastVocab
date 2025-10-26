using System.Net;
using System.Net.Http.Json;
using FastVocab.Shared.DTOs.Words;
using FastVocab.Shared.DTOs.Topics;
using FluentAssertions;
using FastVocab.Test.Shared.Setups;

namespace FastVocab.Test.IntegrationTests;

/// <summary>
/// Integration tests for Words API endpoints
/// Tests the full request/response pipeline including validation, database, and middleware
/// </summary>
public class WordsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public WordsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.InitializeDatabase();
        _client = factory.CreateClient();
    }

    #region Create Word Tests

    [Fact]
    public async Task CreateWord_WithValidData_ShouldReturnCreatedWord()
    {
        // Arrange
        var request = new CreateWordRequest
        {
            Text = "hello",
            Meaning = "xin chào",
            Definition = "A greeting used when meeting someone",
            Type = "Noun",
            Level = "A1",
            Example1 = "Hello, how are you?",
            Example2 = "Hello world!",
            ImageUrl = "https://example.com/hello.jpg",
            AudioUrl = "https://example.com/hello.mp3"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/words", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var word = await response.Content.ReadFromJsonAsync<WordDto>();
        word.Should().NotBeNull();
        word!.Text.Should().Be("hello");
        word.Meaning.Should().Be("xin chào");
        word.Type.Should().Be("Noun");
        word.Level.Should().Be("A1");
    }

    [Fact]
    public async Task CreateWord_WithEmptyText_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateWordRequest
        {
            Text = "",
            Meaning = "Test",
            Type = "Noun",
            Level = "A1"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/words", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadAsStringAsync();
        error.Should().Contain("Text");
    }

    [Fact]
    public async Task CreateWord_WithInvalidType_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateWordRequest
        {
            Text = "test",
            Meaning = "Test",
            Type = "InvalidType",
            Level = "A1"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/words", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadAsStringAsync();
        error.Should().Contain("Type");
    }

    [Fact]
    public async Task CreateWord_WithInvalidLevel_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateWordRequest
        {
            Text = "test",
            Meaning = "Test",
            Type = "Noun",
            Level = "InvalidLevel"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/words", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadAsStringAsync();
        error.Should().Contain("Level");
    }

    [Fact]
    public async Task CreateWord_WithTopics_ShouldReturnCreatedWordWithTopics()
    {
        // Arrange - Create topics first
        var topic1Id = await CreateTopicAsync("Grammar", "Ngữ pháp");
        var topic2Id = await CreateTopicAsync("Vocabulary", "Từ vựng");

        var request = new CreateWordRequest
        {
            Text = "beautiful",
            Meaning = "đẹp",
            Type = "Adjective",
            Level = "B1",
            TopicIds = new List<int> { topic1Id, topic2Id }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/words", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var word = await response.Content.ReadFromJsonAsync<WordDto>();
        word.Should().NotBeNull();
        word!.Text.Should().Be("beautiful");
    }

    #endregion

    #region Get Words Tests

    [Fact]
    public async Task GetAllWords_ShouldReturnAllWords()
    {
        // Arrange - Create some words
        await CreateWordAsync("apple", "quả táo", "Noun", "A1");
        await CreateWordAsync("book", "cuốn sách", "Noun", "A1");

        // Act
        var response = await _client.GetAsync("/api/words");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var words = await response.Content.ReadFromJsonAsync<List<WordDto>>();
        words.Should().NotBeNull();
        words!.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetWordById_WithValidId_ShouldReturnWord()
    {
        // Arrange
        var wordId = await CreateWordAsync("computer", "máy tính", "Noun", "B1");

        // Act
        var response = await _client.GetAsync($"/api/words/{wordId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var word = await response.Content.ReadFromJsonAsync<WordDto>();
        word.Should().NotBeNull();
        word!.Text.Should().Be("computer");
        word.Meaning.Should().Be("máy tính");
    }

    [Fact]
    public async Task GetWordById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/words/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetWordsByTopic_WithValidTopicId_ShouldReturnWords()
    {
        // Arrange
        var topicId = await CreateTopicAsync("Technology", "Công nghệ");
        var wordId = await CreateWordAsync("software", "phần mềm", "Noun", "B2");
        
        // Add word to topic
        await _client.PostAsync($"/api/words/{wordId}/topics/{topicId}", null);

        // Act
        var response = await _client.GetAsync($"/api/words/topic/{topicId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var words = await response.Content.ReadFromJsonAsync<List<WordDto>>();
        words.Should().NotBeNull();
        words!.Should().Contain(w => w.Text == "software");
    }

    [Fact]
    public async Task GetWordsByTopic_WithInvalidTopicId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/words/topic/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetWordsByLevel_WithValidLevel_ShouldReturnWords()
    {
        // Arrange
        await CreateWordAsync("easy", "dễ", "Adjective", "A1");
        await CreateWordAsync("difficult", "khó", "Adjective", "B2");

        // Act
        var response = await _client.GetAsync("/api/words/level/A1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var words = await response.Content.ReadFromJsonAsync<List<WordDto>>();
        words.Should().NotBeNull();
        words!.Should().Contain(w => w.Text == "easy");
        words!.Should().NotContain(w => w.Text == "difficult");
    }

    [Fact]
    public async Task GetWordsByLevel_WithInvalidLevel_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/words/level/InvalidLevel");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Update Word Tests

    [Fact]
    public async Task UpdateWord_WithValidData_ShouldReturnUpdatedWord()
    {
        // Arrange
        var wordId = await CreateWordAsync("old", "cũ", "Adjective", "A2");
        var updateRequest = new UpdateWordRequest
        {
            Id = wordId,
            Text = "old",
            Meaning = "già, cũ",
            Definition = "Having existed for a long time",
            Type = "Adjective",
            Level = "A2"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/words/{wordId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var word = await response.Content.ReadFromJsonAsync<WordDto>();
        word.Should().NotBeNull();
        word!.Meaning.Should().Be("già, cũ");
    }

    [Fact]
    public async Task UpdateWord_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        var updateRequest = new UpdateWordRequest
        {
            Id = 99999,
            Text = "test",
            Meaning = "Test",
            Type = "Noun",
            Level = "A1"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/words/99999", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateWord_WithEmptyText_ShouldReturnBadRequest()
    {
        // Arrange
        var wordId = await CreateWordAsync("valid", "hợp lệ", "Adjective", "B1");
        var updateRequest = new UpdateWordRequest
        {
            Id = wordId,
            Text = "",
            Meaning = "Test",
            Type = "Noun",
            Level = "A1"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/words/{wordId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Delete Word Tests

    [Fact]
    public async Task DeleteWord_WithValidId_ShouldReturnSuccess()
    {
        // Arrange
        var wordId = await CreateWordAsync("delete", "xóa", "Verb", "B1");

        // Act
        var response = await _client.DeleteAsync($"/api/words/{wordId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/words/{wordId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteWord_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/words/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Topic Association Tests

    [Fact]
    public async Task AddWordToTopic_WithValidIds_ShouldReturnSuccess()
    {
        // Arrange
        var topicId = await CreateTopicAsync("Science", "Khoa học");
        var wordId = await CreateWordAsync("research", "nghiên cứu", "Noun", "B2");

        // Act
        var response = await _client.PostAsync($"/api/words/{wordId}/topics/{topicId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadAsStringAsync();
        result.Should().Contain("successfully");
    }

    [Fact]
    public async Task AddWordToTopic_WithInvalidWordId_ShouldReturnBadRequest()
    {
        // Arrange
        var topicId = await CreateTopicAsync("Math", "Toán học");

        // Act
        var response = await _client.PostAsync($"/api/words/99999/topics/{topicId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddWordToTopic_WithInvalidTopicId_ShouldReturnBadRequest()
    {
        // Arrange
        var wordId = await CreateWordAsync("number", "số", "Noun", "A1");

        // Act
        var response = await _client.PostAsync($"/api/words/{wordId}/topics/99999", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RemoveWordFromTopic_WithValidIds_ShouldReturnSuccess()
    {
        // Arrange
        var topicId = await CreateTopicAsync("History", "Lịch sử");
        var wordId = await CreateWordAsync("ancient", "cổ đại", "Adjective", "B2");
        
        // Add word to topic first
        await _client.PostAsync($"/api/words/{wordId}/topics/{topicId}", null);

        // Act
        var response = await _client.DeleteAsync($"/api/words/{wordId}/topics/{topicId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadAsStringAsync();
        result.Should().Contain("successfully");
    }

    [Fact]
    public async Task RemoveWordFromTopic_WithInvalidWordId_ShouldReturnBadRequest()
    {
        // Arrange
        var topicId = await CreateTopicAsync("Art", "Nghệ thuật");

        // Act
        var response = await _client.DeleteAsync($"/api/words/99999/topics/{topicId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RemoveWordFromTopic_NotAssociated_ShouldReturnBadRequest()
    {
        // Arrange
        var topicId = await CreateTopicAsync("Music", "Âm nhạc");
        var wordId = await CreateWordAsync("paint", "sơn", "Verb", "B1");

        // Act - Try to remove without adding first
        var response = await _client.DeleteAsync($"/api/words/{wordId}/topics/{topicId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var topic = await response.Content.ReadFromJsonAsync<TopicDto>();
        return topic!.Id;
    }

    /// <summary>
    /// Helper method to create a word and return its ID
    /// </summary>
    private async Task<int> CreateWordAsync(string text, string meaning, string type, string level)
    {
        var request = new CreateWordRequest
        {
            Text = text,
            Meaning = meaning,
            Type = type,
            Level = level
        };

        var response = await _client.PostAsJsonAsync("/api/words", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var word = await response.Content.ReadFromJsonAsync<WordDto>();
        return word!.Id;
    }

    #endregion
}
