using System;

namespace mid_assignment.Presentations.Endpoints;

public static class EnpointRegistration
{
    public static void MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        var apiGroup = app.MapGroup("/api").WithOpenApi();

        apiGroup.MapUserEndpoints();
        apiGroup.MapBookEndpoints();
        apiGroup.MapCategoryEndpoints();
        apiGroup.MapBookBorrowingRequestEndpoints();
        apiGroup.MapBookReviewEndpoints();
    }
}
