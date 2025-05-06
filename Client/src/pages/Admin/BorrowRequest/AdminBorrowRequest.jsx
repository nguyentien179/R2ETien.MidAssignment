import React, { useState, useEffect } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

const AdminBookBorrowingRequestPage = () => {
  const [requests, setRequests] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [totalPages, setTotalPages] = useState(1);
  const [filters, setFilters] = useState({
    statusFilter: "",
    sortOrder: "",
    pageNumber: 1,
    pageSize: 10,
  });
  const API_URL = import.meta.env.VITE_API_URL;
  const navigate = useNavigate();

  const fetchRequests = async () => {
    try {
      setLoading(true);
      const params = new URLSearchParams();

      if (filters.statusFilter)
        params.append("statusFilter", filters.statusFilter);
      if (filters.sortOrder) params.append("sortOrder", filters.sortOrder);
      params.append("pageNumber", filters.pageNumber);
      params.append("pageSize", filters.pageSize);

      const response = await axios.get(
        `${API_URL}/requests?${params.toString()}`,
        {
          withCredentials: true,
        }
      );

      setRequests(response.data.items || response.data); // Handle both paginated and non-paginated responses
      setTotalPages(response.data.totalPages || 1); // Default to 1 if not paginated
    } catch (err) {
      if (err.response?.status === 401) {
        navigate("/login");
      } else {
        setError(err.response?.data?.message || "Failed to fetch requests");
      }
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRequests();
  }, [filters]);

  const handleStatusUpdate = async (requestId, newStatus) => {
    const confirmMessage =
      newStatus === "APPROVED"
        ? "Are you sure you want to approve this request?"
        : "Are you sure you want to reject this request?";

    if (!window.confirm(confirmMessage)) return;

    try {
      const endpoint =
        newStatus === "APPROVED"
          ? `approve/${requestId}`
          : `reject/${requestId}`;

      await axios.put(`${API_URL}/requests/${endpoint}`, null, {
        withCredentials: true,
      });

      setRequests((prev) =>
        prev.map((request) =>
          request.requestId === requestId
            ? { ...request, requestStatus: newStatus }
            : request
        )
      );
    } catch (err) {
      setError(err.response?.data?.message || "Failed to update status");
    }
  };

  const handlePageChange = (newPage) => {
    setFilters((prev) => ({
      ...prev,
      pageNumber: newPage,
    }));
  };

  const formatDate = (dateString) => {
    if (!dateString) return "N/A";
    const date = new Date(dateString);
    return date.toLocaleDateString("en-US", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  };

  const getStatusBadge = (status) => {
    const statusMap = {
      WAITING: "bg-yellow-100 text-yellow-800",
      APPROVED: "bg-green-100 text-green-800",
      REJECTED: "bg-red-100 text-red-800",
    };

    return (
      <span className={`px-2 py-1 rounded-full text-sm ${statusMap[status]}`}>
        {status.charAt(0) + status.slice(1).toLowerCase()}
      </span>
    );
  };

  return (
    <div className="p-4">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">Borrowing Requests</h1>
      </div>

      {/* Filters */}
      <div className="bg-white p-4 rounded-lg shadow mb-6">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label className="block text-sm font-medium mb-1">Status</label>
            <select
              name="statusFilter"
              value={filters.statusFilter}
              onChange={(e) =>
                setFilters((prev) => ({
                  ...prev,
                  statusFilter: e.target.value,
                  pageNumber: 1, // Reset to first page when filter changes
                }))
              }
              className="w-full p-2 border rounded"
            >
              <option value="">All Statuses</option>
              <option value="WAITING">Waiting</option>
              <option value="APPROVED">Approved</option>
              <option value="REJECTED">Rejected</option>
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">Sort By</label>
            <select
              name="sortOrder"
              value={filters.sortOrder}
              onChange={(e) =>
                setFilters((prev) => ({
                  ...prev,
                  sortOrder: e.target.value,
                  pageNumber: 1, // Reset to first page when sort changes
                }))
              }
              className="w-full p-2 border rounded"
            >
              <option value="">Default</option>
              <option value="asc">Request Date (Oldest First)</option>
              <option value="desc">Request Date (Newest First)</option>
              <option value="due_asc">Due Date (Soonest First)</option>
              <option value="due_desc">Due Date (Latest First)</option>
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">
              Items Per Page
            </label>
            <select
              name="pageSize"
              value={filters.pageSize}
              onChange={(e) =>
                setFilters((prev) => ({
                  ...prev,
                  pageSize: Number(e.target.value),
                  pageNumber: 1, // Reset to first page when page size changes
                }))
              }
              className="w-full p-2 border rounded"
            >
              <option value="5">5</option>
              <option value="10">10</option>
              <option value="20">20</option>
              <option value="50">50</option>
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
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Requestor
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Approver
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Request Date
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Due Date
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Status
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Books
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {requests.length > 0 ? (
                  requests.map((request) => (
                    <tr key={request.requestId}>
                      <td className="px-6 py-4 whitespace-nowrap">
                        {request.requestorName}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        {request.approverName || "N/A"}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        {formatDate(request.requestedDate)}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        {formatDate(request.dueDate)}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        {getStatusBadge(request.requestStatus)}
                      </td>
                      <td className="px-6 py-4">
                        <ul className="list-disc pl-4">
                          {request.details?.map((detail, index) => (
                            <li key={index}>{detail.bookTitle}</li>
                          ))}
                        </ul>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap space-x-2">
                        {request.requestStatus === "WAITING" && (
                          <>
                            <button
                              onClick={() =>
                                handleStatusUpdate(
                                  request.requestId,
                                  "APPROVED"
                                )
                              }
                              className="text-green-600 hover:text-green-900"
                            >
                              Approve
                            </button>
                            <button
                              onClick={() =>
                                handleStatusUpdate(
                                  request.requestId,
                                  "REJECTED"
                                )
                              }
                              className="text-red-600 hover:text-red-900"
                            >
                              Reject
                            </button>
                          </>
                        )}
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan="7" className="px-6 py-4 text-center">
                      No requests found
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>

          {/* Pagination */}
          {totalPages > 1 && (
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
                  disabled={filters.pageNumber >= totalPages}
                  className="ml-3 relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md bg-white"
                >
                  Next
                </button>
              </div>
              <div className="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
                <div>
                  <p className="text-sm text-gray-700">
                    Showing page{" "}
                    <span className="font-medium">{filters.pageNumber}</span> of{" "}
                    <span className="font-medium">{totalPages}</span>
                  </p>
                </div>
                <div>
                  <nav className="relative z-0 inline-flex rounded-md shadow-sm -space-x-px">
                    <button
                      onClick={() =>
                        handlePageChange(Math.max(1, filters.pageNumber - 1))
                      }
                      disabled={filters.pageNumber === 1}
                      className="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium hover:bg-gray-50 disabled:opacity-50"
                    >
                      Previous
                    </button>
                    {Array.from({ length: totalPages }, (_, i) => i + 1).map(
                      (page) => (
                        <button
                          key={page}
                          onClick={() => handlePageChange(page)}
                          className={`relative inline-flex items-center px-4 py-2 border text-sm font-medium ${
                            filters.pageNumber === page
                              ? "bg-blue-50 border-blue-500 text-blue-600"
                              : "bg-white border-gray-300 text-gray-700 hover:bg-gray-50"
                          }`}
                        >
                          {page}
                        </button>
                      )
                    )}
                    <button
                      onClick={() => handlePageChange(filters.pageNumber + 1)}
                      disabled={filters.pageNumber >= totalPages}
                      className="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium hover:bg-gray-50 disabled:opacity-50"
                    >
                      Next
                    </button>
                  </nav>
                </div>
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default AdminBookBorrowingRequestPage;
