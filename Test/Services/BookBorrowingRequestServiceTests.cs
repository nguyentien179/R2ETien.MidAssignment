using System;
using mid_assignment.Application.Interfaces;
using mid_assignment.Application.Services;
using mid_assignment.Domain.Entities;
using mid_assignment.Domain.Enum;
using mid_assignment.Infrastructure.Data;
using mid_assignment.Infrastructure.Repositories.Interfaces;
using mid_assignment.Presentations.DTO.BookBorrowingRequest;
using mid_assignment.Presentations.DTO.BookBorrowingRequestDetails;
using Moq;

namespace Test.Services;

public class BookBorrowingRequestServiceTests
{
    private readonly Mock<IBookBorrowingRequestRepository> _mockRepository;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IBookRepository> _mockBookRepository;
    private readonly Mock<ApplicationDbContext> _mockDbContext;
    private readonly BookBorrowingRequestService _service;

    public BookBorrowingRequestServiceTests()
    {
        _mockRepository = new Mock<IBookBorrowingRequestRepository>();
        _mockUserService = new Mock<IUserService>();
        _mockBookRepository = new Mock<IBookRepository>();
        _mockDbContext = new Mock<ApplicationDbContext>();

        _service = new BookBorrowingRequestService(
            _mockRepository.Object,
            _mockUserService.Object,
            _mockBookRepository.Object,
            _mockDbContext.Object
        );
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_ShouldCreateBorrowingRequest()
    {
        // Arrange
        var dto = new CreateBookBorrowingRequestDTO(
            Guid.NewGuid(),
            [new CreateBookBorrowingRequestDetailsDTO(Guid.NewGuid())]
        );
        var currentUserId = Guid.NewGuid();
        _mockUserService.Setup(u => u.GetCurrentUserId()).Returns(currentUserId);

        // Assuming the book is available for borrowing
        _mockBookRepository
            .Setup(b => b.GetBooksByIdsAsync(It.IsAny<List<Guid>>()))
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

        // Act
        await _service.CreateAsync(dto);

        // Assert
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<BookBorrowingRequest>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ValidRequest_ShouldReturnBookBorrowingRequestDTO()
    {
        // Arrange
        var requestId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();
        _mockUserService.Setup(u => u.GetCurrentUserId()).Returns(currentUserId);

        var request = new BookBorrowingRequest
        {
            RequestorId = currentUserId,
            BorrowingRequestDetails = new List<BookBorrowingRequestDetails>()
        };
        _mockRepository.Setup(r => r.GetByIdAsync(requestId)).ReturnsAsync(request);

        // Act
        var result = await _service.GetByIdAsync(requestId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.RequestorId, result.RequestorId);
    }

    [Fact]
    public async Task CreateAsync_MoreThanFiveBooks_ShouldThrowException()
    {
        // Arrange
        var dto = new CreateBookBorrowingRequestDTO(
            Guid.NewGuid(),
            [
                new CreateBookBorrowingRequestDetailsDTO(Guid.NewGuid()),
                new CreateBookBorrowingRequestDetailsDTO(Guid.NewGuid()),
                new CreateBookBorrowingRequestDetailsDTO(Guid.NewGuid()),
                new CreateBookBorrowingRequestDetailsDTO(Guid.NewGuid()),
                new CreateBookBorrowingRequestDetailsDTO(Guid.NewGuid()),
                new CreateBookBorrowingRequestDetailsDTO(Guid.NewGuid())
            ]
        );
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task UpdateRequestStatusAsync_ValidRequest_ApprovedStatus_ShouldUpdateStatus()
    {
        // Arrange
        var requestId = Guid.NewGuid();
        var approverId = Guid.NewGuid();
        _mockUserService.Setup(u => u.GetCurrentUserId()).Returns(approverId);

        var request = new BookBorrowingRequest
        {
            RequestStatus = RequestStatus.WAITING,
            RequestorId = Guid.NewGuid(),
            ApproverId = null,
            BorrowingRequestDetails = new List<BookBorrowingRequestDetails>
            {
                new BookBorrowingRequestDetails { BookId = Guid.NewGuid() }
            }
        };

        _mockRepository.Setup(r => r.GetByIdAsync(requestId)).ReturnsAsync(request);

        // Act
        await _service.UpdateRequestStatusAsync(requestId, RequestStatus.APPROVED);

        // Assert
        Assert.Equal(RequestStatus.APPROVED, request.RequestStatus);
        Assert.NotNull(request.DueDate);
        _mockRepository.Verify(r => r.Update(It.IsAny<BookBorrowingRequest>()), Times.Once);
    }
    [Fact]
    public async Task UpdateRequestStatusAsync_RequestAlreadyProcessed_ShouldThrowException()
    {
        // Arrange
        var requestId = Guid.NewGuid();
        var approverId = Guid.NewGuid();
        _mockUserService.Setup(u => u.GetCurrentUserId()).Returns(approverId);

        var request = new BookBorrowingRequest
        {
            RequestStatus = RequestStatus.APPROVED,
            RequestorId = Guid.NewGuid(),
            ApproverId = approverId,
            BorrowingRequestDetails = new List<BookBorrowingRequestDetails>
            {
                new BookBorrowingRequestDetails { BookId = Guid.NewGuid() }
            }
        };

        _mockRepository.Setup(r => r.GetByIdAsync(requestId)).ReturnsAsync(request);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateRequestStatusAsync(requestId, RequestStatus.REJECTED));
    }

    [Fact]
    public async Task DeleteAsync_UnauthorizedUser_ShouldThrowException()
    {
        // Arrange
        var requestId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();
        _mockUserService.Setup(u => u.GetCurrentUserId()).Returns(currentUserId);

        var request = new BookBorrowingRequest
        {
            RequestorId = Guid.NewGuid(),
            BorrowingRequestDetails = new List<BookBorrowingRequestDetails>
            {
                new BookBorrowingRequestDetails { BookId = Guid.NewGuid() }
            }
        };

        _mockRepository.Setup(r => r.GetByIdAsync(requestId)).ReturnsAsync(request);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.DeleteAsync(requestId));
    }
    [Fact]
    public async Task ExtendDueDate_ValidRequest_ShouldExtendDueDate()
    {
        // Arrange
        var requestId = Guid.NewGuid();
        var request = new BookBorrowingRequest
        {
            DueDate = DateOnly.FromDateTime(DateTime.Today),
            Extended = false,
            BorrowingRequestDetails = new List<BookBorrowingRequestDetails>
            {
                new BookBorrowingRequestDetails { BookId = Guid.NewGuid() }
            }
        };

        _mockRepository.Setup(r => r.GetByIdAsync(requestId)).ReturnsAsync(request);

        // Act
        await _service.ExtendDueDate(requestId);

        // Assert
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today.AddDays(5)), request.DueDate);
        Assert.True(request.Extended);
        _mockRepository.Verify(r => r.Update(It.IsAny<BookBorrowingRequest>()), Times.Once);
    }
}
