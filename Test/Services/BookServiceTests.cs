using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using mid_assignment.Application.Common.Constants;
using mid_assignment.Application.Interfaces;
using mid_assignment.Application.Services;
using mid_assignment.Domain.Entities;
using mid_assignment.Infrastructure.Repositories.Interfaces;
using mid_assignment.Presentations.DTO.Book;
using Moq;

namespace Test.Services;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _bookRepoMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepoMock = new();
    private readonly Mock<IImageUploader> _imageUploaderMock = new();
    private readonly BookService _service;

    public BookServiceTests()
    {
        _service = new BookService(
            _bookRepoMock.Object,
            _categoryRepoMock.Object,
            _imageUploaderMock.Object
        );
    }

    private IFormFile CreateMockFormFile(string fileName, string contentType, byte[] content)
    {
        var stream = new MemoryStream(content);
        return new FormFile(stream, 0, content.Length, "image", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    [Fact]
    public async Task CreateAsync_Should_Add_Book_When_Valid()
    {
        // Arrange
        var dto = new CreateBookInputDTO(
            "Test Book",
            "Author",
            Guid.NewGuid(),
            10,
            CreateMockFormFile("image.jpg", "image/jpeg", new byte[] { 1, 2, 3 })
        );

        _bookRepoMock
            .Setup(r =>
                r.GetAllAsync(
                    It.IsAny<List<Expression<Func<Book, bool>>>>(),
                    It.IsAny<Func<IQueryable<Book>, IOrderedQueryable<Book>>>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()
                )
            )
            .ReturnsAsync(new List<Book>());
        _categoryRepoMock
            .Setup(r => r.GetByIdAsync(dto.CategoryId))
            .ReturnsAsync(new Category { Name = "Category" });
        _imageUploaderMock.Setup(i => i.UploadImageAsync(dto.Image)).ReturnsAsync("image_url");

        // Act
        await _service.CreateAsync(dto);

        // Assert
        _bookRepoMock.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Once);
        _bookRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_When_Name_Taken()
    {
        // Arrange
        var dto = new CreateBookInputDTO(
            "Duplicate Book",
            "Author",
            Guid.NewGuid(),
            10,
            CreateMockFormFile("image.jpg", "image/jpeg", new byte[] { 1, 2, 3 })
        );

        _bookRepoMock
            .Setup(r =>
                r.GetAllAsync(
                    It.IsAny<List<Expression<Func<Book, bool>>>>(),
                    It.IsAny<Func<IQueryable<Book>, IOrderedQueryable<Book>>>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()
                )
            )
            .ReturnsAsync(
                new List<Book>
                {
                    new Book
                    {
                        Name = "Duplicate Book",
                        Author = "test",
                        Quantity = 1,
                        ImageUrl = "asd"
                    }
                }
            );

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Book()
    {
        var bookId = Guid.NewGuid();
        _bookRepoMock
            .Setup(r => r.GetByIdAsync(bookId))
            .ReturnsAsync(
                new Book
                {
                    Name = " Book",
                    Author = "test",
                    Quantity = 1,
                    ImageUrl = "asd"
                }
            );

        await _service.DeleteAsync(bookId);

        _bookRepoMock.Verify(r => r.Delete(It.Is<Book>(b => b.BookId == bookId)), Times.Once);
        _bookRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Book()
    {
        var bookId = Guid.NewGuid();
        _bookRepoMock
            .Setup(r => r.GetByIdAsync(bookId))
            .ReturnsAsync(
                new Book
                {
                    Name = " Book",
                    Author = "test",
                    Quantity = 1,
                    ImageUrl = "asd"
                }
            );

        var result = await _service.GetByIdAsync(bookId);

        Assert.NotNull(result);
        Assert.Equal("Sample Book", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Fields()
    {
        var bookId = Guid.NewGuid();
        var dto = new UpdateBookDTO(
            "Updated Book",
            "Updated Author",
            Guid.NewGuid(),
            5,
            CreateMockFormFile("image.jpg", "image/jpeg", new byte[] { 1, 2, 3 }),
            "new_image_url"
        );

        var existingBook = new Book
        {
            Name = " Book",
            Author = "test",
            Quantity = 1,
            ImageUrl = "asd"
        };

        _bookRepoMock.Setup(r => r.GetByIdAsync(bookId)).ReturnsAsync(existingBook);
        _bookRepoMock
            .Setup(r =>
                r.GetAllAsync(
                    It.IsAny<List<Expression<Func<Book, bool>>>>(),
                    It.IsAny<Func<IQueryable<Book>, IOrderedQueryable<Book>>>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()
                )
            )
            .ReturnsAsync(new List<Book> { existingBook });
        _categoryRepoMock
            .Setup(r => r.GetByIdAsync(dto.CategoryId))
            .ReturnsAsync(new Category { Name = dto.Name });
        _imageUploaderMock.Setup(i => i.UploadImageAsync(dto.Image)).ReturnsAsync("new_image_url");

        await _service.UpdateAsync(dto, bookId);

        Assert.Equal("Updated Book", existingBook.Name);
        Assert.Equal("Updated Author", existingBook.Author);
        Assert.Equal(dto.CategoryId, existingBook.CategoryId);
        Assert.Equal(5, existingBook.Quantity);
        Assert.Equal("new_image_url", existingBook.ImageUrl);
    }
}
