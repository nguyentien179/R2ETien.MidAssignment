import React, { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import Swal from "sweetalert2";
import { useAuth } from "../../../components/Header";

const AdminUserPage = () => {
  const { authAxios } = useAuth();
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [filters, setFilters] = useState({
    emailFilter: "",
    usernameFilter: "",
    sortOrder: "",
    pageNumber: 1,
    pageSize: 10,
  });
  const navigate = useNavigate();

  const fetchUsers = async () => {
    try {
      setLoading(true);
      const params = new URLSearchParams();

      if (filters.emailFilter)
        params.append("emailFilter", filters.emailFilter);
      if (filters.usernameFilter)
        params.append("usernameFilter", filters.usernameFilter);
      if (filters.sortOrder) params.append("sortOrder", filters.sortOrder);
      params.append("pageNumber", filters.pageNumber);
      params.append("pageSize", filters.pageSize);

      const response = await authAxios.get(`/users?${params.toString()}`);

      setUsers(response.data);
      console.log(response.data);
    } catch (err) {
      if (err.response?.status === 401) {
        navigate("/login");
      } else {
        setError(err.response?.data?.message || "Failed to fetch users");
      }
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, [filters]);

  const handleFilterChange = (e) => {
    const { name, value } = e.target;
    setFilters((prev) => ({
      ...prev,
      [name]: value,
      pageNumber: 1,
    }));
  };

  const handlePageChange = (newPage) => {
    setFilters((prev) => ({
      ...prev,
      pageNumber: newPage,
    }));
  };

  const handleDelete = async (userId) => {
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
      await authAxios.delete(`/users/${userId}`);

      setUsers(users.filter((user) => user.Id !== userId));
      alert("User deleted successfully");
    } catch (err) {
      if (err.response?.status === 401) {
        navigate("/login");
      } else {
        alert("Failed to delete User");
        console.error("Delete error:", err);
      }
    }
  };

  const formatGender = (gender) => {
    const genderMap = {
      0: "Male",
      1: "Female",
      2: "Other",
    };
    return genderMap[gender] || "Unknown";
  };

  const formatRole = (role) => {
    const roleMap = {
      0: "User",
      1: "Admin",
    };
    return roleMap[role] || "Unknown";
  };

  return (
    <div className="p-4">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">User Management</h1>
        <button
          className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
          onClick={() => navigate("/admin/users/add")}
        >
          Add New User
        </button>
      </div>

      <div className="bg-white p-4 rounded-lg shadow mb-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-4">
          <div>
            <label className="block text-sm font-medium mb-1">
              Username Filter
            </label>
            <input
              type="text"
              name="usernameFilter"
              value={filters.usernameFilter}
              onChange={handleFilterChange}
              className="w-full p-2 border rounded"
              placeholder="Filter by username"
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">
              Email Filter
            </label>
            <input
              type="text"
              name="emailFilter"
              value={filters.emailFilter}
              onChange={handleFilterChange}
              className="w-full p-2 border rounded"
              placeholder="Filter by email"
            />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Sort By</label>
            <select
              name="sortOrder"
              value={filters.sortOrder}
              onChange={handleFilterChange}
              className="w-full p-2 border rounded"
            >
              <option value="">Default</option>
              <option value="asc">Username (A-Z)</option>
              <option value="desc">Username (Z-A)</option>
              <option value="email_asc">Email (A-Z)</option>
              <option value="email_desc">Email (Z-A)</option>
            </select>
          </div>
        </div>
      </div>

      {error && (
        <div className="mb-4 p-3 bg-red-100 text-red-700 rounded">{error}</div>
      )}

      {loading ? (
        <div className="flex justify-center items-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-blue-500"></div>
        </div>
      ) : (
        <div className="bg-white rounded-lg shadow overflow-hidden">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Username
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Email
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Gender
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Role
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {users.map((user) => (
                <tr key={user.id}>
                  <td className="px-6 py-4 whitespace-nowrap">
                    {user.username}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">{user.email}</td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    {formatGender(user.gender)}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    {formatRole(user.role)}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap space-x-2">
                    <button
                      onClick={() => navigate(`/admin/users/${user.id}/edit`)}
                      className="text-blue-600 hover:text-blue-900"
                    >
                      Edit
                    </button>
                    <button
                      onClick={() => handleDelete(user.id)}
                      className="text-red-600 hover:text-red-900"
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          <div className="bg-white px-4 py-3 flex items-center justify-between border-t border-gray-200 sm:px-6">
            <div className="flex-1 flex justify-between sm:hidden">
              <button
                onClick={() =>
                  handlePageChange(Math.max(1, filters.pageNumber - 1))
                }
                disabled={filters.pageNumber === 1}
                className="relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md bg-white"
              >
                Previous
              </button>
              <button
                onClick={() => handlePageChange(filters.pageNumber + 1)}
                className="ml-3 relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md bg-white"
              >
                Next
              </button>
            </div>
            <div className="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
              <div>
                <p className="text-sm text-gray-700">
                  Page <span className="font-medium">{filters.pageNumber}</span>
                </p>
              </div>
              <div>
                <nav className="relative z-0 inline-flex rounded-md shadow-sm -space-x-px">
                  <button
                    onClick={() =>
                      handlePageChange(Math.max(1, filters.pageNumber - 1))
                    }
                    disabled={filters.pageNumber === 1}
                    className="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium hover:bg-gray-50"
                  >
                    Previous
                  </button>
                  <button
                    onClick={() => handlePageChange(filters.pageNumber + 1)}
                    className="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium hover:bg-gray-50"
                  >
                    Next
                  </button>
                </nav>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default AdminUserPage;
