using System;

namespace mid_assignment.Application.Common.Constants;

public static class ErrorMessages
{
    // General
    public const string Required = "This field is required.";
    public const string InvalidFormat = "The format is invalid.";
    public const string NotFound = "The requested resource was not found.";
    public const string Unauthorized = "You are not authorized to perform this action.";
    public const string Forbidden = "You do not have permission to perform this action.";
    public const string Conflict = "A conflict occurred with the current state of the resource.";

    // User
    public const string InvalidCredentials = "Invalid username or password.";
    public const string PasswordTooShort = "Password must be at least 8 characters.";
    public const string PasswordTooWeak =
        "Password must contain uppercase, lowercase, a number, and a symbol.";
    public const string EmailInvalid = "Please enter a valid email address.";
    public const string UsernameTaken = "This username is already taken.";
    public const string EmailTaken = "This email is already registered.";
    public const string UserNotFound = "User with this id is not found";

    // Book
    public const string BookNameRequired = "Book name is required.";
    public const string AuthorNameRequired = "Author name is required.";
    public const string CategoryInvalid = "Selected category does not exist.";
    public const string BookNotFound = "Book not found";
    public const string BookNameTaken = "Book name is taken";

    //Category
    public const string CategoryNameExist = "Category with this name already exist";
    public const string CategoryNotFound = "Category with this id not found";

    // Validation
    public const string ValidationFailed = "Request validation failed.";
    public const string InvalidGuid = "Invalid identifier provided.";

    //Request
    public const string RequestNotFound = "Request with this id not found";
    public const string DueDateExtended = "The due date for this request is already extended";
    public const string RequestProcessed = "This request is already processed";
    public const string NotRequestOwner = "You are not the owner of this request";

    //Review
    public const string ReviewNotFound = "Review with this ID is not found";
}
