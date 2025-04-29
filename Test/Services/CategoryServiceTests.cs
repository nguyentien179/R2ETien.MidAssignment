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
    private readonly Mock<ICategoryRepository> _categoryRepoMock = new();
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _categoryService = new CategoryService(_categoryRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidInput_CreatesCategory()
    {
        // Arrange
        var dto = new CreateCategoryDTO("Fiction");
        var category = new Category { CategoryId = Guid.NewGuid(), Name = dto.Name };

        _categoryRepoMock.Setup(x => x.AddAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
        _categoryRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _categoryService.CreateAsync(dto);

        // Assert
        _categoryRepoMock.Verify(
            x => x.AddAsync(It.Is<Category>(c => c.Name == dto.Name)),
            Times.Once
        );
        _categoryRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DuplicateCategoryName_ThrowsInvalidOperationException()
    {
        // Arrange
        var dto = new CreateCategoryDTO("Fiction");
        var categoryId = Guid.NewGuid();
        var existingCategory = new Category { CategoryId = categoryId, Name = "Fiction" };

        _categoryRepoMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Category> { existingCategory });

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _categoryService.CreateAsync(dto)
        );
        Assert.Equal(ErrorMessages.CategoryNameExist, ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_ValidInput_UpdatesCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var dto = new UpdateCategoryDTO(categoryId, "Updated Name");
        var existingCategory = new Category { CategoryId = categoryId, Name = "Old Name" };

        _categoryRepoMock.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync(existingCategory);
        _categoryRepoMock.Setup(x => x.Update(It.IsAny<Category>()));
        _categoryRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _categoryService.UpdateAsync(dto, categoryId);

        // Assert
        _categoryRepoMock.Verify(
            x => x.Update(It.Is<Category>(c => c.CategoryId == categoryId && c.Name == dto.Name)),
            Times.Once
        );
        _categoryRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_CategoryNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var dto = new UpdateCategoryDTO(categoryId, "New Category");

        _categoryRepoMock.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync((Category?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _categoryService.UpdateAsync(dto, categoryId)
        );
        Assert.Equal(ErrorMessages.NotFound, ex.Message);
    }

    [Fact]
    public async Task DeleteAsync_ValidCategory_DeletesCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category { CategoryId = categoryId, Name = "Fiction" };

        _categoryRepoMock.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync(category);
        _categoryRepoMock.Setup(x => x.Delete(category));
        _categoryRepoMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _categoryService.DeleteAsync(categoryId);

        // Assert
        _categoryRepoMock.Verify(x => x.Delete(category), Times.Once);
        _categoryRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_CategoryNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _categoryRepoMock.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync((Category?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _categoryService.DeleteAsync(categoryId)
        );
        Assert.Equal(ErrorMessages.NotFound, ex.Message);
    }

    [Fact]
    public async Task GetByIdAsync_ValidCategory_ReturnsCategory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category { CategoryId = categoryId, Name = "Fiction" };

        _categoryRepoMock.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync(category);

        // Act
        var result = await _categoryService.GetByIdAsync(categoryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryId, result.CategoryId);
        Assert.Equal("Fiction", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_CategoryNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _categoryRepoMock.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync((Category?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _categoryService.GetByIdAsync(categoryId)
        );
        Assert.Equal(ErrorMessages.NotFound, ex.Message);
    }
}
