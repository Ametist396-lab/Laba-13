using System.Net;
using System.Net.Http.Json;
using Laba_13.Models;

namespace Laba_13.Tests.E2ETests;

public class BooksE2ETests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BooksE2ETests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllBooks_ReturnsEmptyArray_WhenNoBooksExist()
    {
        var response = await _client.GetAsync("/api/books");
        var books = await response.Content.ReadFromJsonAsync<List<Book>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(books);
    }

    [Fact]
    public async Task CreateBook_ReturnsCreatedBook_WithId()
    {
        var newBook = new Book
        {
            Name = "War and Peace",
            Author = "Leo Tolstoy",
            ReleaseDate = new DateOnly(1869, 1, 1)
        };

        var response = await _client.PostAsJsonAsync("/api/books", newBook);
        var createdBook = await response.Content.ReadFromJsonAsync<Book>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotEqual(0, createdBook.Id);
        Assert.Equal("War and Peace", createdBook.Name);
        Assert.Equal("Leo Tolstoy", createdBook.Author);
        Assert.Equal(new DateOnly(1869, 1, 1), createdBook.ReleaseDate);
    }

    [Fact]
    public async Task GetBookById_ReturnsBook_WhenBookExists()
    {
        var newBook = new Book
        {
            Name = "1984",
            Author = "George Orwell",
            ReleaseDate = new DateOnly(1949, 6, 8)
        };

        var createResponse = await _client.PostAsJsonAsync("/api/books", newBook);
        var createdBook = await createResponse.Content.ReadFromJsonAsync<Book>();

        var response = await _client.GetAsync($"/api/books/{createdBook.Id}");
        var book = await response.Content.ReadFromJsonAsync<Book>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(createdBook.Id, book.Id);
        Assert.Equal("1984", book.Name);
    }

    [Fact]
    public async Task GetBookById_ReturnsNotFound_WhenBookDoesNotExist()
    {
        var response = await _client.GetAsync("/api/books/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBook_ReturnsNoContent_WhenUpdateSucceeds()
    {
        var newBook = new Book
        {
            Name = "Old Title",
            Author = "Old Author",
            ReleaseDate = new DateOnly(2000, 1, 1)
        };

        var createResponse = await _client.PostAsJsonAsync("/api/books", newBook);
        var createdBook = await createResponse.Content.ReadFromJsonAsync<Book>();

        createdBook.Name = "New Title";
        createdBook.Author = "New Author";

        var response = await _client.PutAsJsonAsync($"/api/books/{createdBook.Id}", createdBook);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/books/{createdBook.Id}");
        var updatedBook = await getResponse.Content.ReadFromJsonAsync<Book>();
        Assert.Equal("New Title", updatedBook.Name);
        Assert.Equal("New Author", updatedBook.Author);
    }

    [Fact]
    public async Task UpdateBook_ReturnsBadRequest_WhenIdMismatch()
    {
        var book = new Book
        {
            Id = 1,
            Name = "Test",
            Author = "Test",
            ReleaseDate = new DateOnly(2020, 1, 1)
        };

        var response = await _client.PutAsJsonAsync("/api/books/999", book);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBook_ReturnsNoContent_WhenDeleteSucceeds()
    {
        var newBook = new Book
        {
            Name = "To Delete",
            Author = "Someone",
            ReleaseDate = new DateOnly(2023, 1, 1)
        };

        var createResponse = await _client.PostAsJsonAsync("/api/books", newBook);
        var createdBook = await createResponse.Content.ReadFromJsonAsync<Book>();

        var response = await _client.DeleteAsync($"/api/books/{createdBook.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/books/{createdBook.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteBook_ReturnsNotFound_WhenBookDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/books/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateBook_AllowsNullReleaseDate()
    {
        var newBook = new Book
        {
            Name = "Null Date Book",
            Author = "Mystery Author",
            ReleaseDate = null
        };

        var response = await _client.PostAsJsonAsync("/api/books", newBook);
        var createdBook = await response.Content.ReadFromJsonAsync<Book>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Null(createdBook.ReleaseDate);
    }
}