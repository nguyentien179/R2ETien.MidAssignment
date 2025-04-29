using System;
using mid_assignment.Application.Common.Constants;
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
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        _bookService = new BookService(_bookRepoMock.Object, _categoryRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidInput_CallsRepository()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var dto = new CreateBookDTO("Book Name", "Author Name", categoryId);
        var category = new Category { CategoryId = categoryId, Name = "Fiction" };

        _categoryRepoMock.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync(category);
        _bookRepoMock.Setup(x => x.AddAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);
        _bookRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _bookService.CreateAsync(dto);

        // Assert
        _bookRepoMock.Verify(
            x =>
                x.AddAsync(
                    It.Is<Book>(b =>
                        b.Name == dto.Name
                        && b.Author == dto.Author
                        && b.CategoryId == dto.CategoryId
                    )
                ),
            Times.Once
        );

        _bookRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_CategoryNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var dto = new CreateBookDTO("Book Title", "Author", categoryId);

        _categoryRepoMock.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync((Category?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _bookService.CreateAsync(dto)
        );
        Assert.Equal(ErrorMessages.NotFound, ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_ValidInput_UpdatesBook()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var dto = new UpdateBookDTO(bookId, "Updated Title", "Updated Author", categoryId);
        var existingBook = new Book
        {
            BookId = bookId,
            Name = "Old",
            Author = "Old",
            CategoryId = categoryId,
        };
        var category = new Category { CategoryId = categoryId, Name = "Category" };

        _bookRepoMock.Setup(x => x.GetByIdAsync(bookId)).ReturnsAsync(existingBook);
        _categoryRepoMock.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync(category);
        _bookRepoMock.Setup(x => x.Update(It.IsAny<Book>()));
        _bookRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _bookService.UpdateAsync(dto, bookId);

        // Assert
        _bookRepoMock.Verify(
            x =>
                x.Update(
                    It.Is<Book>(b =>
                        b.BookId == bookId
                        && b.Name == dto.Name
                        && b.Author == dto.Author
                        && b.CategoryId == dto.CategoryId
                    )
                ),
            Times.Once
        );

        _bookRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_BookNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var dto = new UpdateBookDTO(bookId, "Name", "Author", Guid.NewGuid());

        _bookRepoMock.Setup(x => x.GetByIdAsync(bookId)).ReturnsAsync((Book?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _bookService.UpdateAsync(dto, bookId)
        );
        Assert.Equal(ErrorMessages.NotFound, ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_CategoryNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var dto = new UpdateBookDTO(bookId, "Title", "Author", categoryId);
        var existingBook = new Book
        {
            BookId = bookId,
            Name = "Old",
            Author = "Old",
            CategoryId = Guid.NewGuid(),
        };

        _bookRepoMock.Setup(x => x.GetByIdAsync(bookId)).ReturnsAsync(existingBook);
        _categoryRepoMock.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync((Category?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _bookService.UpdateAsync(dto, bookId)
        );
        Assert.Equal(ErrorMessages.NotFound, ex.Message);
    }

    [Fact]
    public async Task DeleteAsync_ValidBook_DeletesBook()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = new Book
        {
            BookId = bookId,
            Name = "abc",
            Author = "abc",
        };

        _bookRepoMock.Setup(x => x.GetByIdAsync(bookId)).ReturnsAsync(book);
        _bookRepoMock.Setup(x => x.Delete(book));
        _bookRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _bookService.DeleteAsync(bookId);

        // Assert
        _bookRepoMock.Verify(x => x.Delete(book), Times.Once);
        _bookRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
