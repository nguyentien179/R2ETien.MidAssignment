import React, { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

const AdminAddCategoryPage = () => {
  const [formData, setFormData] = useState({
    name: "",
  });
  const [errors, setErrors] = useState({
    name: "",
    description: "",
    general: "",
  });
  const [loading, setLoading] = useState(false);
  const API_URL = import.meta.env.VITE_API_URL;
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
    // Clear error when user types
    setErrors((prev) => ({ ...prev, [name]: "" }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setErrors({ name: "", description: "", general: "" });

    try {
      const response = await axios.post(`${API_URL}/category`, formData, {
        withCredentials: true,
        headers: {
          "Content-Type": "application/json",
        },
      });

      if (response.data.success === false) {
        throw new Error(response.data.message || "Failed to add category");
      }

      alert("Category added successfully!");
      navigate("/admin/categories");
    } catch (err) {
      if (err.response?.status === 401) {
        navigate("/login");
      } else if (err.response?.data?.errors) {
        setErrors((prev) => ({
          ...prev,
          ...err.response.data.errors,
        }));
      } else if (err.response?.data?.message) {
        setErrors((prev) => ({
          ...prev,
          general: err.response.data.message,
        }));
      } else {
        setErrors((prev) => ({
          ...prev,
          general: err.message || "Failed to add category",
        }));
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="max-w-md mx-auto p-6 bg-white rounded-lg shadow-md">
      <h1 className="text-2xl font-bold mb-6">Add New Category</h1>

      {errors.general && (
        <div className="mb-4 p-3 bg-red-100 text-red-700 rounded border border-red-200">
          {errors.general}
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label htmlFor="name" className="block text-sm font-medium mb-1">
            Category Name *
          </label>
          <input
            type="text"
            id="name"
            name="name"
            value={formData.name}
            onChange={handleChange}
            className={`w-full p-2 border rounded focus:ring-2 ${
              errors.name
                ? "border-red-500 focus:ring-red-500"
                : "focus:ring-blue-500"
            }`}
            required
            minLength={2}
            maxLength={50}
          />
          {errors.name && (
            <p className="mt-1 text-sm text-red-600">{errors.name}</p>
          )}
        </div>

        <div className="flex justify-end space-x-3 pt-4">
          <button
            type="button"
            onClick={() => navigate("/admin/categories")}
            className="px-4 py-2 border rounded hover:bg-gray-100"
            disabled={loading}
          >
            Cancel
          </button>
          <button
            type="submit"
            className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:bg-blue-400"
            disabled={loading}
          >
            {loading ? (
              <span className="flex items-center">
                <svg
                  className="animate-spin -ml-1 mr-2 h-4 w-4 text-white"
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                >
                  <circle
                    className="opacity-25"
                    cx="12"
                    cy="12"
                    r="10"
                    stroke="currentColor"
                    strokeWidth="4"
                  ></circle>
                  <path
                    className="opacity-75"
                    fill="currentColor"
                    d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                  ></path>
                </svg>
                Adding...
              </span>
            ) : (
              "Add Category"
            )}
          </button>
        </div>
      </form>
    </div>
  );
};

export default AdminAddCategoryPage;
