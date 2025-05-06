using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mid_assignment.Application.Interfaces;
using mid_assignment.Application.Services;
using mid_assignment.Domain.Entities;
using mid_assignment.Infrastructure.Repositories.Interfaces;
using mid_assignment.Presentations.DTO.BookReview;
using Moq;
using Xunit;

namespace Test.Services;

public class BookReviewServiceTests
{
    private readonly Mock<IBookReviewRepository> _reviewRepoMock;
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly BookReviewService _service;

    public BookReviewServiceTests()
    {
        _reviewRepoMock = new Mock<IBookReviewRepository>();
        _bookRepoMock = new Mock<IBookRepository>();
        _userServiceMock = new Mock<IUserService>();

        _service = new BookReviewService(
            _reviewRepoMock.Object,
            _bookRepoMock.Object,
            _userServiceMock.Object
        );
    }

    [Fact]
    public async Task CreateAsync_Should_Add_Review_When_User_LoggedIn()
    {
        var dto = new CreateBookReviewDTO(5, "Great!");
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _userServiceMock.Setup(u => u.GetCurrentUserId()).Returns(userId);

        await _service.CreateAsync(dto, bookId);

        _reviewRepoMock.Verify(
            r =>
                r.AddAsync(
                    It.Is<BookReview>(br =>
                        br.Rating == dto.Rating
                        && br.Comment == dto.Comment
                        && br.BookId == bookId
                        && br.UserId == userId
                    )
                ),
            Times.Once
        );

        _reviewRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_When_User_NotLoggedIn()
    {
        var dto = new CreateBookReviewDTO(5, "Great!");
        _userServiceMock.Setup(u => u.GetCurrentUserId()).Returns((Guid?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.CreateAsync(dto, Guid.NewGuid())
        );
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_When_Owner()
    {
        var reviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var review = new BookReview { BookReviewId = reviewId, UserId = userId };

        _reviewRepoMock.Setup(r => r.GetByIdAsync(reviewId)).ReturnsAsync(review);
        _userServiceMock.Setup(u => u.GetCurrentUserId()).Returns(userId);
        _userServiceMock.Setup(u => u.IsAdmin()).Returns(false);

        await _service.DeleteAsync(reviewId);

        _reviewRepoMock.Verify(r => r.Delete(review), Times.Once);
        _reviewRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Throw_When_NotOwner_And_NotAdmin()
    {
        var reviewId = Guid.NewGuid();
        var review = new BookReview { BookReviewId = reviewId, UserId = Guid.NewGuid() };

        _reviewRepoMock.Setup(r => r.GetByIdAsync(reviewId)).ReturnsAsync(review);
        _userServiceMock.Setup(u => u.GetCurrentUserId()).Returns(Guid.NewGuid());
        _userServiceMock.Setup(u => u.IsAdmin()).Returns(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.DeleteAsync(reviewId));
    }

    [Fact]
    public async Task GetAllByBookIdAsync_Should_Return_Reviews()
    {
        var bookId = Guid.NewGuid();
        var book = new Book
        {
            Name = "Duplicate Book",
            Author = "test",
            Quantity = 1,
            ImageUrl = "asd"
        };
        var reviews = new List<BookReview>
        {
            new BookReview { Rating = 4, Comment = "Nice" }
        };

        _bookRepoMock.Setup(b => b.GetByIdAsync(bookId)).ReturnsAsync(book);
        _reviewRepoMock.Setup(r => r.GetAllByBookIdAsync(bookId)).ReturnsAsync(reviews);

        var result = await _service.GetAllByBookIdAsync(bookId);

        Assert.Single(result);
        Assert.Equal(4, result.First().Rating);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Review()
    {
        var reviewId = Guid.NewGuid();
        var review = new BookReview { BookReviewId = reviewId, Rating = 3 };

        _reviewRepoMock.Setup(r => r.GetByIdAsync(reviewId)).ReturnsAsync(review);

        var result = await _service.GetByIdAsync(reviewId);

        Assert.NotNull(result);
        Assert.Equal(3, result!.Rating);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Review_When_Owner()
    {
        var reviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dto = new UpdateBookReviewDTO(Guid.NewGuid(), 1, "x", Guid.NewGuid(), Guid.NewGuid());
        var review = new BookReview { BookReviewId = reviewId, UserId = userId };

        _reviewRepoMock.Setup(r => r.GetByIdAsync(reviewId)).ReturnsAsync(review);
        _userServiceMock.Setup(u => u.GetCurrentUserId()).Returns(userId);

        await _service.UpdateAsync(reviewId, dto);

        Assert.Equal(dto.Rating, review.Rating);
        Assert.Equal(dto.Comment, review.Comment);
        _reviewRepoMock.Verify(r => r.Update(review), Times.Once);
        _reviewRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_When_NotOwner()
    {
        var reviewId = Guid.NewGuid();
        var review = new BookReview { BookReviewId = reviewId, UserId = Guid.NewGuid() };

        _reviewRepoMock.Setup(r => r.GetByIdAsync(reviewId)).ReturnsAsync(review);
        _userServiceMock.Setup(u => u.GetCurrentUserId()).Returns(reviewId);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () =>
                _service.UpdateAsync(
                    reviewId,
                    new UpdateBookReviewDTO(Guid.NewGuid(), 1, "x", Guid.NewGuid(), Guid.NewGuid())
                )
        );
    }
}
