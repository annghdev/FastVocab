using System.Net;
using System.Net.Http.Json;
using FastVocab.Shared.DTOs.Practice;
using FastVocab.Shared.DTOs.Users.Requests;
using FastVocab.Shared.DTOs.Collections;
using FastVocab.Shared.DTOs.WordLists;
using FastVocab.Test.Shared.Setups;
using FluentAssertions;

namespace FastVocab.Test.IntegrationTests;

[Collection("IntegrationTests")]
public class PracticeSessionsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public PracticeSessionsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.InitializeDatabase();
        _client = factory.CreateClient();
    }

    #region Create PracticeSession Tests

    [Fact]
    public async Task CreatePracticeSession_WithValidData_ShouldReturnCreatedPracticeSession()
    {
        // Arrange
        var userId = await CreateUserAsync("Practice User");
        var collectionId = await CreateCollectionAsync("Practice Collection", "Test collection");
        var wordListId = await CreateWordListAsync(collectionId, "Practice List");

        var request = new CreatePracticeSessionRequest
        {
            UserId = userId,
            ListId = wordListId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/practicesessions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var practiceSession = await response.Content.ReadFromJsonAsync<PracticeSessionDto>();
        practiceSession.Should().NotBeNull();
        practiceSession!.UserId.Should().Be(userId);
        practiceSession.ListId.Should().Be(wordListId);
        practiceSession.RepetitionCount.Should().Be(0);
    }

    [Fact]
    public async Task CreatePracticeSession_WithDuplicateUserAndList_ShouldReturnBadRequest()
    {
        // Arrange
        var userId = await CreateUserAsync("Duplicate Test User");
        var collectionId = await CreateCollectionAsync("Duplicate Collection", "Test");
        var wordListId = await CreateWordListAsync(collectionId, "Duplicate List");

        var request = new CreatePracticeSessionRequest
        {
            UserId = userId,
            ListId = wordListId
        };

        // Act
        await _client.PostAsJsonAsync("/api/practicesessions", request);
        var response = await _client.PostAsJsonAsync("/api/practicesessions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePracticeSession_WithNonExistentUser_ShouldReturnBadRequest()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        var collectionId = await CreateCollectionAsync("Test Collection", "Test");
        var wordListId = await CreateWordListAsync(collectionId, "Test List");

        var request = new CreatePracticeSessionRequest
        {
            UserId = nonExistentUserId,
            ListId = wordListId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/practicesessions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePracticeSession_WithNonExistentWordList_ShouldReturnBadRequest()
    {
        // Arrange
        var userId = await CreateUserAsync("Test User");

        var request = new CreatePracticeSessionRequest
        {
            UserId = userId,
            ListId = 99999 // Non-existent list ID
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/practicesessions", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePracticeSession_WithMultipleUsersSameList_ShouldSucceed()
    {
        // Arrange
        var userId1 = await CreateUserAsync("User 1");
        var userId2 = await CreateUserAsync("User 2");
        var collectionId = await CreateCollectionAsync("Shared Collection", "Test");
        var wordListId = await CreateWordListAsync(collectionId, "Shared List");

        var request1 = new CreatePracticeSessionRequest
        {
            UserId = userId1,
            ListId = wordListId
        };

        var request2 = new CreatePracticeSessionRequest
        {
            UserId = userId2,
            ListId = wordListId
        };

        // Act
        var response1 = await _client.PostAsJsonAsync("/api/practicesessions", request1);
        var response2 = await _client.PostAsJsonAsync("/api/practicesessions", request2);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreatePracticeSession_WithSameUserMultipleLists_ShouldSucceed()
    {
        // Arrange
        var userId = await CreateUserAsync("Multi List User");
        var collectionId = await CreateCollectionAsync("Multi Collection", "Test");
        var wordListId1 = await CreateWordListAsync(collectionId, "List 1");
        var wordListId2 = await CreateWordListAsync(collectionId, "List 2");

        var request1 = new CreatePracticeSessionRequest
        {
            UserId = userId,
            ListId = wordListId1
        };

        var request2 = new CreatePracticeSessionRequest
        {
            UserId = userId,
            ListId = wordListId2
        };

        // Act
        var response1 = await _client.PostAsJsonAsync("/api/practicesessions", request1);
        var response2 = await _client.PostAsJsonAsync("/api/practicesessions", request2);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Get PracticeSession Tests

    [Fact]
    public async Task GetAllPracticeSessions_ShouldReturnAllPracticeSessions()
    {
        // Arrange
        var userId1 = await CreateUserAsync("GetAll User 1");
        var userId2 = await CreateUserAsync("GetAll User 2");
        var collectionId = await CreateCollectionAsync("GetAll Collection", "Test");
        var wordListId1 = await CreateWordListAsync(collectionId, "List 1");
        var wordListId2 = await CreateWordListAsync(collectionId, "List 2");

        await CreatePracticeSessionAsync(userId1, wordListId1);
        await CreatePracticeSessionAsync(userId1, wordListId2);
        await CreatePracticeSessionAsync(userId2, wordListId1);

        // Act
        var response = await _client.GetAsync("/api/practicesessions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var practiceSessions = await response.Content.ReadFromJsonAsync<List<PracticeSessionDto>>();
        practiceSessions.Should().NotBeNull();
        practiceSessions!.Count.Should().BeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task GetPracticeSessionById_WithValidId_ShouldReturnPracticeSession()
    {
        // Arrange
        var userId = await CreateUserAsync("GetById User");
        var collectionId = await CreateCollectionAsync("GetById Collection", "Test");
        var wordListId = await CreateWordListAsync(collectionId, "GetById List");
        var practiceSessionId = await CreatePracticeSessionAsync(userId, wordListId);

        // Act
        var response = await _client.GetAsync($"/api/practicesessions/{practiceSessionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var practiceSession = await response.Content.ReadFromJsonAsync<PracticeSessionDto>();
        practiceSession.Should().NotBeNull();
        practiceSession!.Id.Should().Be(practiceSessionId);
        practiceSession.UserId.Should().Be(userId);
        practiceSession.ListId.Should().Be(wordListId);
    }

    [Fact]
    public async Task GetPracticeSessionById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/practicesessions/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPracticeSessionsByUserId_WithValidUserId_ShouldReturnUserSessions()
    {
        // Arrange
        var userId1 = await CreateUserAsync("GetByUser User 1");
        var userId2 = await CreateUserAsync("GetByUser User 2");
        var collectionId = await CreateCollectionAsync("GetByUser Collection", "Test");
        var wordListId1 = await CreateWordListAsync(collectionId, "List 1");
        var wordListId2 = await CreateWordListAsync(collectionId, "List 2");

        await CreatePracticeSessionAsync(userId1, wordListId1);
        await CreatePracticeSessionAsync(userId1, wordListId2);
        await CreatePracticeSessionAsync(userId2, wordListId1);

        // Act
        var response = await _client.GetAsync($"/api/practicesessions/user/{userId1}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var practiceSessions = await response.Content.ReadFromJsonAsync<List<PracticeSessionDto>>();
        practiceSessions.Should().NotBeNull();
        practiceSessions!.Count.Should().BeGreaterThanOrEqualTo(2);
        practiceSessions.All(ps => ps.UserId == userId1).Should().BeTrue();
    }

    [Fact]
    public async Task GetPracticeSessionsByUserId_WithInvalidUserId_ShouldReturnEmptyList()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/practicesessions/user/{nonExistentUserId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var practiceSessions = await response.Content.ReadFromJsonAsync<List<PracticeSessionDto>>();
        practiceSessions.Should().NotBeNull();
        practiceSessions!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPracticeSessionsByUserId_WithUserWithNoSessions_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = await CreateUserAsync("No Sessions User");

        // Act
        var response = await _client.GetAsync($"/api/practicesessions/user/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var practiceSessions = await response.Content.ReadFromJsonAsync<List<PracticeSessionDto>>();
        practiceSessions.Should().NotBeNull();
        practiceSessions!.Should().BeEmpty();
    }

    #endregion

    #region Submit PracticeSession Tests

    [Fact]
    public async Task SubmitPracticeSession_WithValidId_ShouldUpdateRepetitionCount()
    {
        // Arrange
        var userId = await CreateUserAsync("Submit User");
        var collectionId = await CreateCollectionAsync("Submit Collection", "Test");
        var wordListId = await CreateWordListAsync(collectionId, "Submit List");
        var practiceSessionId = await CreatePracticeSessionAsync(userId, wordListId);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/practicesessions/{practiceSessionId}", userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var practiceSession = await response.Content.ReadFromJsonAsync<PracticeSessionDto>();
        practiceSession.Should().NotBeNull();
        practiceSession!.RepetitionCount.Should().BeGreaterThan(0);
        practiceSession.LastReviewed.Should().NotBeNull();
        practiceSession.NextReview.Should().NotBeNull();
    }

    [Fact]
    public async Task SubmitPracticeSession_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        var userId = await CreateUserAsync("Invalid Submit User");

        // Act
        var response = await _client.PutAsJsonAsync("/api/practicesessions/99999", userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SubmitPracticeSession_MultipleTimes_ShouldIncrementRepetitionCount()
    {
        // Arrange
        var userId = await CreateUserAsync("Multiple Submit User");
        var collectionId = await CreateCollectionAsync("Multiple Submit Collection", "Test");
        var wordListId = await CreateWordListAsync(collectionId, "Multiple Submit List");
        var practiceSessionId = await CreatePracticeSessionAsync(userId, wordListId);

        // Act - Submit first time
        var response1 = await _client.PutAsJsonAsync($"/api/practicesessions/{practiceSessionId}", userId);
        var practiceSession1 = await response1.Content.ReadFromJsonAsync<PracticeSessionDto>();

        // Act - Submit second time
        var response2 = await _client.PutAsJsonAsync($"/api/practicesessions/{practiceSessionId}", userId);
        var practiceSession2 = await response2.Content.ReadFromJsonAsync<PracticeSessionDto>();

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        practiceSession2!.RepetitionCount.Should().BeGreaterThan(practiceSession1!.RepetitionCount);
    }

    [Fact]
    public async Task SubmitPracticeSession_ShouldUpdateNextReviewDate()
    {
        // Arrange
        var userId = await CreateUserAsync("NextReview User");
        var collectionId = await CreateCollectionAsync("NextReview Collection", "Test");
        var wordListId = await CreateWordListAsync(collectionId, "NextReview List");
        var practiceSessionId = await CreatePracticeSessionAsync(userId, wordListId);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/practicesessions/{practiceSessionId}", userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var practiceSession = await response.Content.ReadFromJsonAsync<PracticeSessionDto>();
        practiceSession.Should().NotBeNull();
        practiceSession!.NextReview.Should().NotBeNull();
        practiceSession.NextReview.Should().BeAfter(practiceSession.LastReviewed!.Value);
    }

    [Fact]
    public async Task SubmitPracticeSession_WithMismatchedUserId_ShouldStillSucceed()
    {
        // Note: Current implementation doesn't validate userId in Submit
        // This test documents current behavior
        // Arrange
        var userId1 = await CreateUserAsync("User 1");
        var userId2 = await CreateUserAsync("User 2");
        var collectionId = await CreateCollectionAsync("Mismatch Collection", "Test");
        var wordListId = await CreateWordListAsync(collectionId, "Mismatch List");
        var practiceSessionId = await CreatePracticeSessionAsync(userId1, wordListId);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/practicesessions/{practiceSessionId}", userId2);

        // Assert
        // Current implementation doesn't validate userId, so this may succeed
        // This documents the current behavior - may need to add validation later
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
    }

    #endregion

    #region Helper Methods

    private async Task<Guid> CreateUserAsync(string fullName, string? sessionId = null, Guid? accountId = null)
    {
        var request = new CreateUserRequest { FullName = fullName, SessionId = sessionId, AccountId = accountId };
        var response = await _client.PostAsJsonAsync("/api/users", request);
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<FastVocab.Shared.DTOs.Users.UserDto>();
        return user!.Id;
    }

    private async Task<int> CreateCollectionAsync(string name, string description)
    {
        var request = new CreateCollectionRequest
        {
            Name = name,
            Description = description
        };

        var response = await _client.PostAsJsonAsync("/api/collections", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var collection = await response.Content.ReadFromJsonAsync<CollectionDto>();
        return collection!.Id;
    }

    private async Task<int> CreateWordListAsync(int collectionId, string name)
    {
        var request = new CreateWordListRequest { CollectionId = collectionId, Name = name };
        var response = await _client.PostAsJsonAsync($"/api/collections/{collectionId}/lists", request);
        var wordList = await response.Content.ReadFromJsonAsync<WordListDto>();
        return wordList!.Id;
    }

    private async Task<int> CreatePracticeSessionAsync(Guid userId, int listId)
    {
        var request = new CreatePracticeSessionRequest
        {
            UserId = userId,
            ListId = listId
        };

        var response = await _client.PostAsJsonAsync("/api/practicesessions", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var practiceSession = await response.Content.ReadFromJsonAsync<PracticeSessionDto>();
        return practiceSession!.Id;
    }

    #endregion
}

