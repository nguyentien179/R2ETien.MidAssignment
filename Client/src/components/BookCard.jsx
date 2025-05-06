import { useNavigate } from "react-router-dom";

const BookCard = ({ book, onAddToCart }) => {
  return (
    <div className="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-transform hover:-translate-y-1 h-full flex flex-col">
      <div className="h-48 bg-gray-200 flex items-center justify-center">
        {book.imageUrl ? (
          <img
            src={book.imageUrl}
            alt={book.title}
            className="h-full w-full object-cover"
            onClick={() => useNavigate(`/book/${book.bookId}`)}
            onError={(e) => {
              e.target.onerror = null;
              e.target.src = "/book-placeholder.png";
            }}
          />
        ) : (
          <div className="text-gray-500 p-4 text-center">
            <span className="text-4xl">📚</span>
            <p>No cover available</p>
          </div>
        )}
      </div>
      <div className="p-4 flex-grow flex flex-col">
        <h3 className="text-xl font-semibold mb-2 line-clamp-2">{book.name}</h3>
        <p className="text-gray-600 mb-2">by {book.author}</p>
        <p className="text-sm text-gray-500 mb-2">{book.category}</p>
        <div className="mt-auto pt-4">
          <button
            onClick={() => onAddToCart(book)}
            className="w-full bg-blue-600 text-white py-2 px-4 rounded hover:bg-blue-700 transition"
          >
            Add to Borrow List
          </button>
        </div>
      </div>
    </div>
  );
};

export default BookCard;
