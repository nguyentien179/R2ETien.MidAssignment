using System;
using mid_assignment.Application.Common.Constants;
using mid_assignment.Application.Services;
using mid_assignment.Domain.Entities;
using mid_assignment.Infrastructure.Repositories.Interfaces;
using mid_assignment.Presentations.DTO.Category;
using Moq;

namespace Test.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _categoryRepoMock;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _service = new CategoryService(_categoryRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_Should_Add_Category_When_Valid()
    {
        // Arrange
        var dto = new CreateCategoryDTO("NewCategory");
        _categoryRepoMock
            .Setup(r => r.GetAllAsync(null, null, null, null, null))
            .ReturnsAsync(new List<Category>());

        // Act
        await _service.CreateAsync(dto);

        // Assert
        _categoryRepoMock.Verify(
            r => r.AddAsync(It.Is<Category>(c => c.Name == dto.Name)),
            Times.Once
        );
        _categoryRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_When_Name_Exists()
    {
        var dto = new CreateCategoryDTO("Existing");
        var existing = new List<Category> { new Category { Name = "existing" } };

        _categoryRepoMock
            .Setup(r => r.GetAllAsync(null, null, null, null, null))
            .ReturnsAsync(existing);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task DeleteAsync_Should_Throw_If_Category_Has_Books()
    {
        var category = new Category
        {
            Name = "sdasd",
            Books = new List<Book>
            {
                new Book
                {
                    Name = " Book",
                    Author = "test",
                    Quantity = 1,
                    ImageUrl = "asd"
                }
            }
        };

        _categoryRepoMock.Setup(r => r.GetByIdAsync(category.CategoryId)).ReturnsAsync(category);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.DeleteAsync(category.CategoryId)
        );
        Assert.Contains("books", ex.Message);
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_When_Valid()
    {
        var id = Guid.NewGuid();
        var category = new Category { Name = "asd", Books = new List<Book>() };

        _categoryRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(category);

        await _service.DeleteAsync(id);

        _categoryRepoMock.Verify(
            r => r.Delete(It.Is<Category>(c => c.CategoryId == id)),
            Times.Once
        );
        _categoryRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_Categories()
    {
        var categories = new List<Category> { new Category { Name = "Test" } };

        _categoryRepoMock
            .Setup(r => r.GetAllAsync(null, null, null, It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(categories);

        var result = await _service.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("Test", result.First().Name);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Category()
    {
        var id = Guid.NewGuid();
        var category = new Category { Name = "Sample" };

        _categoryRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(category);

        var result = await _service.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("Sample", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_When_Name_Exists()
    {
        var id = Guid.NewGuid();
        var dto = new UpdateCategoryDTO(id, "Duplicate");
        var existing = new List<Category> { new Category { Name = "duplicate" } };

        _categoryRepoMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(new Category { Name = "asf" });
        _categoryRepoMock
            .Setup(r => r.GetAllAsync(null, null, null, null, null))
            .ReturnsAsync(existing);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(dto, id));
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_When_Valid()
    {
        var id = Guid.NewGuid();
        var dto = new UpdateCategoryDTO(id, "Updated");
        var category = new Category { Name = "Old" };

        _categoryRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(category);
        _categoryRepoMock
            .Setup(r => r.GetAllAsync(null, null, null, null, null))
            .ReturnsAsync(new List<Category>());

        await _service.UpdateAsync(dto, id);

        _categoryRepoMock.Verify(
            r => r.Update(It.Is<Category>(c => c.Name == dto.Name)),
            Times.Once
        );
        _categoryRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
