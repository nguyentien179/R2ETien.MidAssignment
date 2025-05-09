import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import axios from "axios";
import { useAuth } from "../../../components/Header";

const AdminUpdateBookPage = () => {
  const { authAxios } = useAuth();
  const { id } = useParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [categories, setCategories] = useState([]);

  const API_URL = import.meta.env.VITE_API_URL;
  const [formData, setFormData] = useState({
    name: "",
    author: "",
    quantity: 0,
    categoryId: "",
    image: null,
    existingImage: "",
  });

  const fetchExistingImageBlob = async (url) => {
    const response = await fetch(url);
    const blob = await response.blob();
    return new File([blob], "existing.jpg", { type: blob.type });
  };

  useEffect(() => {
    const fetchData = async () => {
      try {
        const bookResponse = await authAxios.get(`/books/${id}`);
        console.log(bookResponse.data);

        const categoriesResponse = await authAxios.get(`/category`);

        setFormData({
          name: bookResponse.data.name,
          author: bookResponse.data.author,
          quantity: bookResponse.data.quantity,
          categoryId: bookResponse.data.categoryId,
          existingImage: bookResponse.data.imageUrl,
        });

        setCategories(categoriesResponse.data);
        console.log(categoriesResponse.data);
      } catch (err) {
        if (err.response?.status === 401) {
          navigate("/login");
        } else {
          setError("Failed to fetch book data");
          console.log(err.response?.data?.message || err.message);
        }
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [id, authAxios, navigate]);

  const handleInputChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
  };

  const handleFileChange = (e) => {
    setFormData({
      ...formData,
      image: e.target.files[0],
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    let imageToSend = formData.image;
    try {
      const formDataToSend = new FormData();
      formDataToSend.append("Name", formData.name);
      formDataToSend.append("Author", formData.author);
      formDataToSend.append("Quantity", formData.quantity);
      formDataToSend.append("categoryId", formData.categoryId);
      if (!imageToSend && formData.existingImage) {
        imageToSend = await fetchExistingImageBlob(formData.existingImage);
      }

      if (imageToSend) {
        formDataToSend.append("Image", imageToSend);
      }

      await authAxios.put(`/books/${id}`, formDataToSend, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });

      navigate("/admin/books");
    } catch (err) {
      if (err.response?.status === 401) {
        navigate("/login");
      } else {
        setError(err.response?.data?.message || "Failed to update book");
      }
    }
  };

  if (loading) {
    return <div className="p-4">Loading...</div>;
  }

  return (
    <div className="max-w-2xl mx-auto p-4">
      <h1 className="text-2xl font-bold mb-6">Update Book</h1>

      {error && <div className="mb-4 p-2 bg-red-100 text-red-700">{error}</div>}

      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="block text-sm font-medium mb-1">Title</label>
          <input
            type="text"
            name="name"
            value={formData.name}
            onChange={handleInputChange}
            className="w-full p-2 border rounded"
            required
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Author</label>
          <input
            type="text"
            name="author"
            value={formData.author}
            onChange={handleInputChange}
            className="w-full p-2 border rounded"
            required
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Quantity</label>
          <input
            type="number"
            name="quantity"
            value={formData.quantity}
            onChange={handleInputChange}
            className="w-full p-2 border rounded"
            min="0"
            required
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Category</label>
          <select
            name="categoryId"
            value={formData.categoryId}
            onChange={handleInputChange}
            className="w-full p-2 border rounded"
            required
          >
            <option value="">Select a category</option>
            {categories.map((category) => (
              <option key={category.categoryId} value={category.categoryId}>
                {category.name}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Image</label>
          <input
            type="file"
            accept="image/*"
            onChange={handleFileChange}
            className="w-full p-2 border rounded"
          />
          {formData.existingImage && (
            <div className="mt-2">
              <span className="text-sm">Current Image:</span>
              <img
                src={formData.existingImage}
                alt="Current book cover"
                className="mt-1 w-32 h-32 object-cover"
              />
            </div>
          )}
        </div>

        <button
          type="submit"
          className="w-full bg-blue-600 text-white py-2 px-4 rounded hover:bg-blue-700"
        >
          Update Book
        </button>
      </form>
    </div>
  );
};

export default AdminUpdateBookPage;
