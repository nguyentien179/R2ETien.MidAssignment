import React, { useEffect, useState } from "react";
import axios from "axios";
import Swal from "sweetalert2";
import { Link } from "react-router-dom";
import { useAuth } from "../../../components/Header";

const AdminBooksPage = () => {
  const { authAxios } = useAuth();
  const [books, setBooks] = useState([]);
  const [nameFilter, setNameFilter] = useState("");
  const [authorFilter, setAuthorFilter] = useState("");
  const [categoryFilter, setCategoryFilter] = useState("");
  const [sortOrder, setSortOrder] = useState("");
  const [categories, setCategories] = useState([]);

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const response = await authAxios.get(`/category`);
        setCategories(response.data);
        console.log(response.data);
      } catch (error) {
        console.error("Failed to fetch categories", error);
      }
    };

    fetchCategories();
  }, [authAxios]);

  useEffect(() => {
    const fetchBooks = async () => {
      try {
        const params = new URLSearchParams();
        if (nameFilter) {
          params.append("nameFilter", nameFilter);
        }
        if (authorFilter) {
          params.append("authorFilter", authorFilter);
        }
        if (categoryFilter) {
          const selectedCategory = categories.find(
            (cat) => cat.name === categoryFilter
          );
          if (selectedCategory?.categoryId) {
            params.append("categoryId", selectedCategory.categoryId);
          }
        }
        if (sortOrder) {
          params.append("sortOrder", sortOrder);
        }

        const response = await authAxios.get(`/books?${params.toString()}`, {
          withCredentials: true,
        });
        setBooks(response.data);
        console.log(response.data);
      } catch (error) {
        console.error("Failed to fetch books", error);
      }
    };

    fetchBooks();
  }, [
    authAxios,
    nameFilter,
    authorFilter,
    categoryFilter,
    sortOrder,
    categories,
  ]);

  const handleCategoryChange = (event) => {
    setCategoryFilter(event.target.value);
  };

  const handleSortChange = (event) => {
    setSortOrder(event.target.value);
  };

  const handleDelete = async (bookId) => {
    const result = await Swal.fire({
      title: "Are you sure?",
      text: "You won't be able to revert this!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#d33",
      confirmButtonText: "Yes, delete it!",
    });

    if (!result.isConfirmed) return;

    try {
      await authAxios.delete(`/books/${bookId}`);
      const updatedBooks = books.filter((book) => book.bookId !== bookId);
      setBooks(updatedBooks);
    } catch (error) {
      if (error.response?.status === 401) {
        navigate("/login");
      } else {
        console.error("Failed to delete book", error);
        alert("Failed to delete book. Please try again.");
      }
    }
  };

  return (
    <div className="p-4">
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-semibold">Books</h1>
        <Link
          to="/admin/books/add"
          className="bg-green-500 hover:bg-green-700 text-white font-bold py-2 px-4 rounded"
        >
          Add Book
        </Link>
      </div>

      <div className="mb-4 flex items-center space-x-4">
        <div className="flex items-center">
          <label htmlFor="nameSearch" className="mr-2">
            Title:
          </label>
          <input
            type="text"
            id="nameSearch"
            className="border border-gray-300 rounded-md p-2"
            placeholder="Title"
            value={nameFilter}
            onChange={(e) => setNameFilter(e.target.value)}
          />
        </div>

        <div className="flex items-center ml-4">
          <label htmlFor="authorSearch" className="mr-2">
            Author:
          </label>
          <input
            type="text"
            id="authorSearch"
            className="border border-gray-300 rounded-md p-2"
            placeholder="Author"
            value={authorFilter}
            onChange={(e) => setAuthorFilter(e.target.value)}
          />
        </div>

        <div>
          <label htmlFor="category" className="mr-2">
            Category:
          </label>
          <select
            id="category"
            className="border border-gray-300 rounded-md p-2"
            value={categoryFilter}
            onChange={handleCategoryChange}
          >
            <option value="">All Categories</option>
            {categories.map((category) => (
              <option key={category.categoryId} value={category.name}>
                {category.name}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label htmlFor="sort" className="mr-2">
            Sort By:
          </label>
          <select
            id="sort"
            className="border border-gray-300 rounded-md p-2"
            value={sortOrder}
            onChange={handleSortChange}
          >
            <option value="">Default</option>
            <option value="asc">Title (A-Z)</option>
            <option value="desc">Title (Z-A)</option>
            <option value="author_asc">Author (A-Z)</option>
            <option value="author_desc">Author (Z-A)</option>
          </select>
        </div>
      </div>

      <div className="overflow-x-auto">
        <table className="min-w-full bg-white border border-gray-200">
          <thead className="bg-gray-100 text-left">
            <tr>
              <th className="p-2 w-3/12">Title</th>
              <th className="p-2 w-3/12">Author</th>
              <th className="p-2 w-1/12">Category</th>
              <th className="p-2 w-1/12">Quantity</th>
              <th className="p-2 w-1/12">Actions</th>
            </tr>
          </thead>
          <tbody>
            {books.map((book) => (
              <tr key={book.id} className="border-t border-gray-100">
                <td className="p-2">{book.name}</td>
                <td className="p-2">{book.author}</td>
                <td className="p-2">{book.categoryName}</td>
                <td className="p-2">{book.quantity}</td>
                <td className="p-2">
                  <Link
                    to={`/admin/books/${book.bookId}/update`}
                    className="text-blue-500 hover:underline"
                  >
                    Edit
                  </Link>
                  <button
                    onClick={() => handleDelete(book.bookId)}
                    className="ml-2 text-red-500 hover:underline"
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}
            {books.length === 0 && (
              <tr>
                <td colSpan="5" className="p-4 text-center text-gray-500">
                  No books available.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default AdminBooksPage;
