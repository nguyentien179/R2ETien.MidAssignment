using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mid_assignment.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fourth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookReview_BookId_UserId",
                table: "BookReview");

            migrationBuilder.CreateIndex(
                name: "IX_BookReview_BookId",
                table: "BookReview",
                column: "BookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookReview_BookId",
                table: "BookReview");

            migrationBuilder.CreateIndex(
                name: "IX_BookReview_BookId_UserId",
                table: "BookReview",
                columns: new[] { "BookId", "UserId" },
                unique: true);
        }
    }
}
