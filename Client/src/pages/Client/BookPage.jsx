import { useState, useEffect, useContext } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { CartContext } from "../../components/Header";
import BookCard from "../../components/BookCard";

const BooksPage = () => {
  const [books, setBooks] = useState([]);
  const [loading, setLoading] = useState(false);
  const [categories, setCategories] = useState([]);
  const [totalPages, setTotalPages] = useState(1);
  const API_URL = import.meta.env.VITE_API_URL;
  const navigate = useNavigate();
  const { addToCart } = useContext(CartContext);

  // Combined filters state
  const [filters, setFilters] = useState({
    nameFilter: "",
    authorFilter: "",
    categoryFilter: "",
    sortOrder: "",
    page: 1,
    pageSize: 12,
  });

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const response = await axios.get(`${API_URL}/category`);
        setCategories(response.data);
      } catch (error) {
        console.error("Failed to fetch categories", error);
      }
    };

    fetchCategories();
  }, [API_URL]);

  useEffect(() => {
    const fetchBooks = async () => {
      try {
        setLoading(true);
        const params = new URLSearchParams();

        // Apply filters
        if (filters.nameFilter) params.append("nameFilter", filters.nameFilter);
        if (filters.authorFilter)
          params.append("authorFilter", filters.authorFilter);
        if (filters.categoryFilter) {
          const selectedCategory = categories.find(
            (cat) => cat.name === filters.categoryFilter
          );
          if (selectedCategory?.categoryId) {
            params.append("categoryId", selectedCategory.categoryId);
          }
        }
        if (filters.sortOrder) params.append("sortOrder", filters.sortOrder);

        // Pagination
        params.append("pageNumber", filters.page);
        params.append("pageSize", filters.pageSize);

        const response = await axios.get(
          `${API_URL}/books?${params.toString()}`,
          { withCredentials: true }
        );

        setBooks(response.data);
      } catch (error) {
        console.error("Failed to fetch books", error);
      } finally {
        setLoading(false);
      }
    };

    fetchBooks();
  }, [API_URL, filters, categories]);

  const handleFilterChange = (e) => {
    const { name, value } = e.target;
    setFilters((prev) => ({
      ...prev,
      [name]: value,
      page: 1,
    }));
  };

  const handlePageChange = (newPage) => {
    setFilters((prev) => ({
      ...prev,
      page: newPage,
    }));
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const handleAddToCart = (book) => {
    addToCart(book);
    alert(`${book.name} added to your borrow list`);
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="container mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold mb-8 text-center">
          Browse Our Collection
        </h1>

        <div className="bg-white p-6 rounded-lg shadow-md mb-8">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            {/* Title Filter */}
            <div className="flex items-center">
              <label htmlFor="nameSearch" className="mr-2">
                Title:
              </label>
              <input
                type="text"
                name="nameFilter"
                className="border border-gray-300 rounded-md p-2"
                placeholder="Title"
                value={filters.nameFilter}
                onChange={handleFilterChange}
              />
            </div>

            {/* Author Filter */}
            <div className="flex items-center">
              <label htmlFor="authorSearch" className="mr-2">
                Author:
              </label>
              <input
                type="text"
                name="authorFilter"
                className="border border-gray-300 rounded-md p-2"
                placeholder="Author"
                value={filters.authorFilter}
                onChange={handleFilterChange}
              />
            </div>

            {/* Category Filter */}
            <div>
              <label htmlFor="category" className="mr-2">
                Category:
              </label>
              <select
                name="categoryFilter"
                className="border border-gray-300 rounded-md p-2"
                value={filters.categoryFilter}
                onChange={handleFilterChange}
              >
                <option value="">All Categories</option>
                {categories.map((category) => (
                  <option key={category.categoryId} value={category.name}>
                    {category.name}
                  </option>
                ))}
              </select>
            </div>

            {/* Sort By */}
            <div>
              <label htmlFor="sort" className="mr-2">
                Sort By:
              </label>
              <select
                name="sortOrder"
                className="border border-gray-300 rounded-md p-2"
                value={filters.sortOrder}
                onChange={handleFilterChange}
              >
                <option value="">Default</option>
                <option value="asc">Title (A-Z)</option>
                <option value="desc">Title (Z-A)</option>
                <option value="author_asc">Author (A-Z)</option>
                <option value="author_desc">Author (Z-A)</option>
              </select>
            </div>
          </div>

          {/* Items Per Page */}
          <div className="mt-4 w-1/4">
            <label className="block text-sm font-medium mb-1">
              Items Per Page
            </label>
            <select
              name="pageSize"
              className="w-full p-2 border rounded focus:ring-2 focus:ring-blue-500"
              value={filters.pageSize}
              onChange={handleFilterChange}
            >
              <option value="12">12</option>
              <option value="24">24</option>
              <option value="48">48</option>
            </select>
          </div>
        </div>

        {/* Books Grid */}
        {loading ? (
          <div className="flex justify-center items-center h-64">
            <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-blue-500"></div>
          </div>
        ) : (
          <>
            {books.length === 0 ? (
              <div className="text-center py-12">
                <p className="text-xl text-gray-600">No books found</p>
                <p className="text-gray-500 mt-2">
                  Try adjusting your search filters
                </p>
              </div>
            ) : (
              <>
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 mb-8">
                  {books.map((book) => (
                    <BookCard
                      key={book.bookId || book.id}
                      book={book}
                      onAddToCart={handleAddToCart}
                    />
                  ))}
                </div>

                {/* Pagination - Only show if totalPages > 1 */}
                {totalPages > 1 && (
                  <div className="flex justify-center mt-8">
                    <nav className="inline-flex rounded-md shadow">
                      <button
                        onClick={() => handlePageChange(filters.page - 1)}
                        disabled={filters.page === 1}
                        className="px-3 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50"
                      >
                        Previous
                      </button>

                      {[...Array(totalPages).keys()].map((num) => (
                        <button
                          key={num + 1}
                          onClick={() => handlePageChange(num + 1)}
                          className={`px-3 py-2 border-t border-b border-gray-300 bg-white text-sm font-medium ${
                            filters.page === num + 1
                              ? "bg-blue-50 text-blue-600 border-blue-500"
                              : "text-gray-700 hover:bg-gray-50"
                          }`}
                        >
                          {num + 1}
                        </button>
                      ))}

                      <button
                        onClick={() => handlePageChange(filters.page + 1)}
                        disabled={filters.page === totalPages}
                        className="px-3 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50"
                      >
                        Next
                      </button>
                    </nav>
                  </div>
                )}
              </>
            )}
          </>
        )}
      </div>
    </div>
  );
};

export default BooksPage;
