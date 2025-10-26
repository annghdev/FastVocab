using System.Net;
using System.Net.Http.Json;
using FastVocab.Shared.DTOs.Users;
using FastVocab.Shared.DTOs.Users.Requests;
using FastVocab.Test.Shared.Setups;
using FluentAssertions;

namespace FastVocab.Test.IntegrationTests;

public class UsersIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public UsersIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.InitializeDatabase();
        _client = factory.CreateClient();
    }

    #region Create User Tests

    [Fact]
    public async Task CreateUser_WithValidData_ShouldReturnCreatedUser()
    {
        // Arrange
        var request = new CreateUserRequest { FullName = "Test User" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user.Should().NotBeNull();
        user!.FullName.Should().Be("Test User");
    }
    
    [Fact]
    public async Task CreateUser_WithAccountId_ShouldSucceed()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            FullName = "Account User",
            AccountId = Guid.NewGuid()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user.Should().NotBeNull();
        user!.AccountId.Should().Be(request.AccountId);
    }

    [Fact]
    public async Task CreateUser_WithDuplicateAccountId_ShouldReturnConflict()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        await CreateUserAsync("User with AccountId", accountId: accountId);

        var request = new CreateUserRequest
        {
            FullName = "Another User",
            AccountId = accountId
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateUser_WithFullNameTooLong_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            FullName = new string('a', 101)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Get Users Tests

    [Fact]
    public async Task GetAllUsers_ShouldReturnAllUsers()
    {
        // Arrange
        await CreateUserAsync("User 1");
        await CreateUserAsync("User 2");

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        users.Should().NotBeNull();
        users!.Count.Should().BeGreaterThanOrEqualTo(2);
    }
    
    [Fact]
    public async Task GetUserBySessionId_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var sessionId = $"sess_{Guid.NewGuid()}";
        await CreateUserAsync("Session User", sessionId);

        // Act
        var response = await _client.GetAsync($"/api/users/session/{sessionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user.Should().NotBeNull();
        user!.FullName.Should().Be("Session User");
    }

    [Fact]
    public async Task GetUserById_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var userId = await CreateUserAsync("GetByIdTest");

        // Act
        var response = await _client.GetAsync($"/api/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user.Should().NotBeNull();
        user!.FullName.Should().Be("GetByIdTest");
    }

    [Fact]
    public async Task GetUserById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/users/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Update User Tests

    [Fact]
    public async Task UpdateUser_WithValidData_ShouldReturnUpdatedUser()
    {
        // Arrange
        var userId = await CreateUserAsync("UpdateTest");
        var updateRequest = new UpdateUserRequest
        {
            Id = userId,
            FullName = "UpdatedName"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/users/{userId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user.Should().NotBeNull();
        user!.FullName.Should().Be("UpdatedName");
    }
    
    [Fact]
    public async Task UpdateUser_WithMismatchedId_ShouldReturnBadRequest()
    {
        // Arrange
        var userId = await CreateUserAsync("UpdateTest");
        var updateRequest = new UpdateUserRequest
        {
            Id = Guid.NewGuid(),
            FullName = "UpdatedName"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/users/{userId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion
    
    #region Delete User Tests

    [Fact]
    public async Task DeleteUser_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var userId = await CreateUserAsync("DeleteTest");

        // Act
        var response = await _client.DeleteAsync($"/api/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/users/{userId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteUser_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync($"/api/users/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Helper Methods

    private async Task<Guid> CreateUserAsync(string fullName, string? sessionId = null, Guid? accountId = null)
    {
        var request = new CreateUserRequest { FullName = fullName, SessionId = sessionId, AccountId = accountId };
        var response = await _client.PostAsJsonAsync("/api/users", request);
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        return user!.Id;
    }

    #endregion
}
