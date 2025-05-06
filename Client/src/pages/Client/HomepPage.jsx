import { useState, useEffect, useContext } from "react";
import axios from "axios";
import { Link, useNavigate } from "react-router-dom";
import Header, { CartContext } from "../../components/Header";

const HomePage = () => {
  const [books, setBooks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [featuredBooks, setFeaturedBooks] = useState([]);
  const API_URL = import.meta.env.VITE_API_URL;
  const { addToCart } = useContext(CartContext);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchBooks = async () => {
      try {
        const [generalResponse, featuredResponse] = await Promise.all([
          axios.get(`${API_URL}/books?pageSize=6`),
          axios.get(`${API_URL}/books?pageSize=3`),
        ]);

        setBooks(generalResponse.data);
        setFeaturedBooks(featuredResponse.data);
      } catch (err) {
        setError(err.response?.data?.message || "Failed to load books");
      } finally {
        setLoading(false);
      }
    };

    fetchBooks();
  }, [API_URL]);

  const handleAddToCart = (book) => {
    addToCart(book);
    alert(`${book.name} added to your borrow list`);
  };

  if (loading) {
    return (
      <div className="min-h-screen">
        <div className="flex justify-center items-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-blue-500"></div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen">
        <div className="p-4 text-red-500">{error}</div>
      </div>
    );
  }

  return (
    <div className="min-h-screen">
      {/* Hero Section */}
      <section className="bg-blue-600 text-white py-20">
        <div className="container mx-auto px-4 text-center">
          <h1 className="text-4xl font-bold mb-4">Welcome to Our Library</h1>
          <p className="text-xl mb-8">
            Discover thousands of books to expand your knowledge
          </p>
          <Link
            to="/books"
            className="bg-white text-blue-600 px-6 py-3 rounded-lg font-medium hover:bg-blue-50 transition"
          >
            Browse All Books
          </Link>
        </div>
      </section>

      {/* Featured Books */}
      <section className="py-16 bg-gray-50">
        <div className="container mx-auto px-4">
          <h2 className="text-3xl font-bold mb-8 text-center">
            Featured Books
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            {featuredBooks.map((book) => (
              <div
                key={book.id}
                className="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition"
              >
                <div className="h-48 bg-gray-200 flex items-center justify-center">
                  {book.imageUrl ? (
                    <img
                      src={book.imageUrl}
                      alt={book.name}
                      className="h-full object-cover"
                      onClick={() => navigate(`/book/${book.bookId}`)}
                    />
                  ) : (
                    <span className="text-gray-500">No Image</span>
                  )}
                </div>
                <div className="p-4">
                  <h3 className="text-xl font-semibold mb-2">{book.name}</h3>
                  <p className="text-gray-600 mb-2">by {book.author}</p>
                  <div className="flex justify-between items-center mt-4">
                    <button
                      onClick={() => handleAddToCart(book)}
                      className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
                    >
                      Add to Borrow List
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Recent Books */}
      <section className="py-16">
        <div className="container mx-auto px-4">
          <h2 className="text-3xl font-bold mb-8 text-center">
            Recently Added
          </h2>
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8">
            {books.map((book) => (
              <div
                key={book.id}
                className="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition"
              >
                <div className="h-48 bg-gray-200 flex items-center justify-center">
                  {book.imageUrl ? (
                    <img
                      src={book.imageUrl}
                      alt={book.name}
                      className="h-full object-cover"
                    />
                  ) : (
                    <span className="text-gray-500">No Image</span>
                  )}
                </div>
                <div className="p-4">
                  <h3 className="text-xl font-semibold mb-2">{book.name}</h3>
                  <p className="text-gray-600 mb-2">by {book.author}</p>
                  <div className="flex justify-between items-center mt-4">
                    <button
                      onClick={() => handleAddToCart(book)}
                      className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
                    >
                      Add to Borrow List
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
          <div className="text-center mt-8">
            <Link
              to="/books"
              className="inline-block bg-blue-600 text-white px-6 py-3 rounded-lg font-medium hover:bg-blue-700 transition"
            >
              View All Books
            </Link>
          </div>
        </div>
      </section>

      {/* About Section */}
      <section className="py-16 bg-gray-50">
        <div className="container mx-auto px-4">
          <div className="max-w-3xl mx-auto text-center">
            <h2 className="text-3xl font-bold mb-6">About Our Library</h2>
            <p className="text-lg text-gray-700 mb-6">
              Our library provides access to thousands of books across various
              genres. Members can borrow up to 5 books at a time for a period of
              2 weeks.
            </p>
            <Link
              to="/about"
              className="inline-block bg-blue-600 text-white px-6 py-3 rounded-lg font-medium hover:bg-blue-700 transition"
            >
              Learn More
            </Link>
          </div>
        </div>
      </section>
    </div>
  );
};

export default HomePage;
