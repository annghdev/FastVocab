using FastVocab.API.Test.Setups;
using FastVocab.Shared.DTOs.Collections;
using FastVocab.Shared.DTOs.WordLists;
using FastVocab.Shared.DTOs.Words;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace FastVocab.API.Test.Integration;

[Collection("IntegrationTests")]
public class CollectionsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public CollectionsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.InitializeDatabase();
        _client = factory.CreateClient();
    }

    #region Create Collection Tests

    [Fact]
    public async Task CreateCollection_WithValidData_ShouldReturnCreatedCollection()
    {
        // Arrange
        var request = new CreateCollectionRequest
        {
            Name = "Test Collection",
            Description = "Test description",
            TargetAudience = "Students",
            DifficultyLevel = "B1",
            ImageUrl = "https://example.com/test.jpg"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/collections", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var collection = await response.Content.ReadFromJsonAsync<CollectionDto>();
        collection.Should().NotBeNull();
        collection!.Name.Should().Be("Test Collection");
        collection.Description.Should().Be("Test description");
    }

    [Fact]
    public async Task CreateCollection_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateCollectionRequest
        {
            Name = "",
            Description = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/collections", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadAsStringAsync();
        error.Should().Contain("Name");
    }

    [Fact]
    public async Task CreateCollection_WithDuplicateName_ShouldReturnBadRequest()
    {
        // Arrange
        var request1 = new CreateCollectionRequest { Name = "Duplicate", Description = "Test 1" };
        var request2 = new CreateCollectionRequest { Name = "Duplicate", Description = "Test 2" };

        // Act
        await _client.PostAsJsonAsync("/api/collections", request1);
        var response = await _client.PostAsJsonAsync("/api/collections", request2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Get Collections Tests

    [Fact]
    public async Task GetAllCollections_ShouldReturnAllCollections()
    {
        // Arrange - Create some collections
        await CreateCollectionAsync("Collection1", "Desc1");
        await CreateCollectionAsync("Collection2", "Desc2");

        // Act
        var response = await _client.GetAsync("/api/collections");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var collections = await response.Content.ReadFromJsonAsync<List<CollectionDto>>();
        collections.Should().NotBeNull();
        collections!.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetCollectionById_WithValidId_ShouldReturnCollection()
    {
        // Arrange
        var collectionId = await CreateCollectionAsync("GetByIdTest", "Test desc");

        // Act
        var response = await _client.GetAsync($"/api/collections/{collectionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var collection = await response.Content.ReadFromJsonAsync<CollectionDto>();
        collection.Should().NotBeNull();
        collection!.Name.Should().Be("GetByIdTest");
    }

    [Fact]
    public async Task GetCollectionById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/collections/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCollectionWithLists_WithValidId_ShouldReturnCollectionWithLists()
    {
        // Arrange
        var collectionId = await CreateCollectionAsync("WithListsTest", "Test");

        // Act
        var response = await _client.GetAsync($"/api/collections/{collectionId}/lists");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var collection = await response.Content.ReadFromJsonAsync<CollectionDto>();
        collection.Should().NotBeNull();
        collection!.WordLists.Should().NotBeNull();
    }

    #endregion

    #region Update Collection Tests

    [Fact]
    public async Task UpdateCollection_WithValidData_ShouldReturnUpdatedCollection()
    {
        // Arrange
        var collectionId = await CreateCollectionAsync("UpdateTest", "Original desc");
        var updateRequest = new UpdateCollectionRequest
        {
            Id = collectionId,
            Name = "Updated Name",
            Description = "Updated desc"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/collections/{collectionId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var collection = await response.Content.ReadFromJsonAsync<CollectionDto>();
        collection.Should().NotBeNull();
        collection!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task UpdateCollection_WithInvalidId_ShouldReturnBadRequest()
    {
        // Arrange
        var updateRequest = new UpdateCollectionRequest { Id = 99999, Name = "Test" };

        // Act
        var response = await _client.PutAsJsonAsync("/api/collections/99999", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Delete Collection Tests

    [Fact]
    public async Task DeleteCollection_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var collectionId = await CreateCollectionAsync("DeleteTest", "Test");

        // Act
        var response = await _client.DeleteAsync($"/api/collections/{collectionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/collections/{collectionId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCollection_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/collections/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Toggle Visibility Tests

    [Fact]
    public async Task ToggleVisibility_WithValidId_ShouldToggleSuccessfully()
    {
        // Arrange
        var collectionId = await CreateCollectionAsync("ToggleTest", "Test");

        // Act - Toggle to hidden
        var response1 = await _client.PatchAsync($"/api/collections/{collectionId}/visibility", null);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        var collection1 = await response1.Content.ReadFromJsonAsync<CollectionDto>();
        collection1!.IsHiding.Should().BeTrue();

        // Act - Toggle back to visible
        var response2 = await _client.PatchAsync($"/api/collections/{collectionId}/visibility", null);

        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var collection2 = await response2.Content.ReadFromJsonAsync<CollectionDto>();
        collection2!.IsHiding.Should().BeFalse();
    }

    [Fact]
    public async Task ToggleVisibility_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.PatchAsync("/api/collections/99999/visibility", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Create WordList Tests

    [Fact]
    public async Task CreateWordList_WithValidData_ShouldReturnCreatedWordList()
    {
        // Arrange
        var collectionId = await CreateCollectionAsync("WordListTest", "Test");
        var request = new CreateWordListRequest
        {
            CollectionId = collectionId,
            Name = "Test List",
            ImageUrl = "https://example.com/list.jpg"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/collections/{collectionId}/lists", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var wordList = await response.Content.ReadFromJsonAsync<WordListDto>();
        wordList.Should().NotBeNull();
        wordList!.Name.Should().Be("Test List");
        wordList.CollectionId.Should().Be(collectionId);
    }

    [Fact]
    public async Task CreateWordList_WithDuplicateName_ShouldReturnBadRequest()
    {
        // Arrange
        var collectionId = await CreateCollectionAsync("DuplicateListTest", "Test");
        var request1 = new CreateWordListRequest { CollectionId = collectionId, Name = "Duplicate" };
        var request2 = new CreateWordListRequest { CollectionId = collectionId, Name = "Duplicate" };

        // Act
        await _client.PostAsJsonAsync($"/api/collections/{collectionId}/lists", request1);
        var response = await _client.PostAsJsonAsync($"/api/collections/{collectionId}/lists", request2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Update WordList Tests

    [Fact]
    public async Task UpdateWordList_WithValidData_ShouldReturnUpdatedWordList()
    {
        // Arrange
        var collectionId = await CreateCollectionAsync("UpdateListTest", "Test");
        var wordListId = await CreateWordListAsync(collectionId, "Original List");
        var updateRequest = new UpdateWordListRequest
        {
            Id = wordListId,
            Name = "Updated List Name",
            ImageUrl = "https://example.com/updated.jpg"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/collections/{collectionId}/lists/{wordListId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var wordList = await response.Content.ReadFromJsonAsync<WordListDto>();
        wordList.Should().NotBeNull();
        wordList!.Name.Should().Be("Updated List Name");
    }

    [Fact]
    public async Task UpdateWordList_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var updateRequest = new UpdateWordListRequest { Id = 99999, Name = "Test" };

        // Act
        var response = await _client.PutAsJsonAsync("/api/collections{1}/lists/99999", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateWordList_WithDuplicateName_ShouldReturnBadRequest()
    {
        // Arrange
        var collectionId = await CreateCollectionAsync("DuplicateListUpdate", "Test");
        await CreateWordListAsync(collectionId, "Existing List");
        var wordListId = await CreateWordListAsync(collectionId, "To Update");
        var updateRequest = new UpdateWordListRequest { Id = wordListId, Name = "Existing List" };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/collections/{collectionId}/lists/{wordListId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Add Word to List Tests

    [Fact]
    public async Task AddWordToList_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var collectionId = await CreateCollectionAsync("AddWordTest", "Test");
        var wordListId = await CreateWordListAsync(collectionId, "Test List");
        var wordId = await CreateWordAsync("TestWord", "Test Meaning"); // Assume helper for creating word

        var request = new AddWordToListRequest { WordListId = wordListId, WordId = wordId };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/collections/{collectionId}/lists/{wordListId}/words", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddWordToList_DuplicateAdd_ShouldReturnBadRequest()
    {
        // temporary fix
        using var factory = new CustomWebApplicationFactory();
        factory.InitializeDatabase();
        _client = factory.CreateClient();

        // Arrange
        var collectionId = await CreateCollectionAsync("DuplicateAddTest", "Test");
        var wordListId = await CreateWordListAsync(collectionId, "Test List");
        var wordId = await CreateWordAsync("TestWord", "Test Meaning");

        var request = new AddWordToListRequest { WordListId = wordListId, WordId = wordId };

        await _client.PostAsJsonAsync($"/api/collections/{collectionId}/lists/{wordListId}/words", request);

        // Act
        var response = await _client.PostAsJsonAsync($"/api/collections/{collectionId}/lists/{wordListId}/words", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Remove Word from List Tests

    [Fact]
    public async Task RemoveWordFromList_WithValidData_ShouldReturnNoContent()
    {
        // temporary fix
        using var factory = new CustomWebApplicationFactory();
        factory.InitializeDatabase();
        _client = factory.CreateClient();

        // Arrange
        var collectionId = await CreateCollectionAsync("RemoveWordTest", "Test");
        var wordListId = await CreateWordListAsync(collectionId, "Test List");
        var wordId = await CreateWordAsync("TestWord", "Test Meaning");

        await AddWordToListAsync(collectionId,wordListId, wordId);

        // Act
        var response = await _client.DeleteAsync($"/api/collections/{collectionId}/lists/{wordListId}/words/{wordId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RemoveWordFromList_NonExistent_ShouldReturnNotFound()
    {
        // Arrange
        var collectionId = await CreateCollectionAsync("RemoveNonExistent", "Test");
        var wordListId = await CreateWordListAsync(collectionId, "Test List");

        // Act
        var response = await _client.DeleteAsync($"/api/collections/lists/{wordListId}/words/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Delete WordList Tests

    [Fact]
    public async Task DeleteWordList_WithValidId_ShouldReturnNoContent()
    {
        // temporary fix
        using var factory = new CustomWebApplicationFactory();
        factory.InitializeDatabase();
        _client = factory.CreateClient();
        // Arrange
        var collectionId = await CreateCollectionAsync("DeleteListTest", "Test");
        var wordListId = await CreateWordListAsync(collectionId, "To Delete");

        // Act
        var response = await _client.DeleteAsync($"/api/collections/{collectionId}/lists/{wordListId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteWordList_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var collectionId = await CreateCollectionAsync("DeleteListTest", "Test");
        // Act
        var response = await _client.DeleteAsync($"/api/collections/{collectionId}/lists/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Helper Methods

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

    private async Task<int> CreateWordAsync(string text, string meaning)
    {
        var request = new CreateWordRequest
        {
            Text = text,
            Meaning = meaning,
            Type = "Noun",
            Level = "A1"
        };
        var response = await _client.PostAsJsonAsync("/api/words", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var word = await response.Content.ReadFromJsonAsync<WordDto>();
        return word!.Id;
    }

    private async Task AddWordToListAsync(int collectionId, int wordListId, int wordId)
    {
        var request = new AddWordToListRequest { WordListId = wordListId, WordId = wordId };
        await _client.PostAsJsonAsync($"/api/collections/{collectionId}/lists/{wordListId}/words", request);
    }

    #endregion
}
