using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mid_assignment.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Category_CategoryId",
                table: "Book");

            migrationBuilder.DropForeignKey(
                name: "FK_BookBorrowingRequestDetails_Book_BookId",
                table: "BookBorrowingRequestDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_BookReview_Book_BookId",
                table: "BookReview");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Book",
                table: "Book");

            migrationBuilder.RenameTable(
                name: "Book",
                newName: "Books");

            migrationBuilder.RenameIndex(
                name: "IX_Book_Name",
                table: "Books",
                newName: "IX_Books_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Book_CategoryId",
                table: "Books",
                newName: "IX_Books_CategoryId");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "BookBorrowingRequest",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Books",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Books",
                table: "Books",
                column: "BookId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Book_Quantity_NonNegative",
                table: "Books",
                sql: "[Quantity] >= 0");

            migrationBuilder.AddForeignKey(
                name: "FK_BookBorrowingRequestDetails_Books_BookId",
                table: "BookBorrowingRequestDetails",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookReview_Books_BookId",
                table: "BookReview",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Category_CategoryId",
                table: "Books",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookBorrowingRequestDetails_Books_BookId",
                table: "BookBorrowingRequestDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_BookReview_Books_BookId",
                table: "BookReview");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Category_CategoryId",
                table: "Books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Books",
                table: "Books");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Book_Quantity_NonNegative",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "BookBorrowingRequest");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Books");

            migrationBuilder.RenameTable(
                name: "Books",
                newName: "Book");

            migrationBuilder.RenameIndex(
                name: "IX_Books_Name",
                table: "Book",
                newName: "IX_Book_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Books_CategoryId",
                table: "Book",
                newName: "IX_Book_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Book",
                table: "Book",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Category_CategoryId",
                table: "Book",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookBorrowingRequestDetails_Book_BookId",
                table: "BookBorrowingRequestDetails",
                column: "BookId",
                principalTable: "Book",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BookReview_Book_BookId",
                table: "BookReview",
                column: "BookId",
                principalTable: "Book",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
