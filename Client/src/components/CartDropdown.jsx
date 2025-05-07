import { useState, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { CartContext, useAuth } from "./Header";
import axios from "axios";
import Swal from "sweetalert2";

const CartDropdown = () => {
  const { authAxios } = useAuth();
  const navigate = useNavigate();
  const { cartItems, removeFromCart, clearCart } = useContext(CartContext);
  const [isCartOpen, setIsCartOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const API_URL = import.meta.env.VITE_API_URL;

  const totalItems = cartItems.reduce((sum, item) => sum + item.quantity, 0);

  const showErrorAlert = (message) => {
    Swal.fire({
      icon: "error",
      title: "Error",
      text: message,
      confirmButtonColor: "#3085d6",
    });
  };

  const handleBorrowRequest = async () => {
    if (totalItems > 5) {
      showErrorAlert("You cannot borrow more than 5 books at a time");
      return;
    }

    setIsLoading(true);
    setError(null);
    try {
      const requestDetails = cartItems.map((item) => ({
        BookId: item.bookId,
        Quantity: item.quantity,
      }));

      const response = await authAxios.post(
        `/requests`,
        {
          Details: requestDetails,
        },
        {
          headers: {
            "Content-Type": "application/json",
          },
        }
      );
      Swal.fire({
        icon: "success",
        title: "Success!",
        text: "Your borrow request has been submitted",
        confirmButtonColor: "#3085d6",
      });
      clearCart();
    } catch (error) {
      console.error("Borrow request failed:", error);

      let errorMessage = "An unexpected error occurred";

      if (error.response) {
        const backendError = error.response.data;

        if (error.response.status === 400) {
          errorMessage =
            backendError.title ||
            "Validation error - please check your request";
        } else if (error.response.status === 404) {
          errorMessage = "One or more books were not found";
        } else if (error.response.status === 409) {
          errorMessage =
            backendError || "Inventory conflict - please try again";
        } else if (error.response.status === 429) {
          errorMessage =
            backendError?.Error ||
            "You've reached your monthly borrow limit (5 requests). Please try again next month.";
        } else if (
          error.response.status === 401 ||
          error.response.status === 403
        ) {
          errorMessage = "Please login to complete your request";
          navigate("/login");
        } else {
          errorMessage = backendError || "Failed to process your request";
        }
      } else if (error.request) {
        errorMessage = "Network error - please check your connection";
      }

      showErrorAlert(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="relative">
      <button
        className="p-2 text-gray-700 hover:text-gray-900 relative"
        onClick={() => setIsCartOpen((prev) => !prev)}
        aria-label="Cart dropdown"
      >
        🛒 Cart
        {totalItems > 0 && (
          <span className="absolute -top-1 -right-1 bg-red-500 text-white rounded-full w-5 h-5 text-xs flex items-center justify-center">
            {totalItems}
          </span>
        )}
      </button>

      {isCartOpen && (
        <div className="absolute right-0 mt-3 w-80 bg-white rounded-2xl shadow-xl border z-50">
          {totalItems > 0 ? (
            <>
              <div className="p-4 border-b">
                <h3 className="text-lg font-semibold text-gray-800">
                  Your Books
                </h3>
                <p className="text-sm text-gray-500">Max 5 books per request</p>
              </div>

              <div className="max-h-64 overflow-y-auto divide-y">
                {cartItems.map((book) => (
                  <div
                    key={book.bookId}
                    className="flex items-center gap-3 px-4 py-3"
                  >
                    <img
                      src={book.imageUrl}
                      alt={book.name}
                      className="w-12 h-12 rounded object-cover border"
                      loading="lazy"
                    />
                    <div className="flex-1">
                      <p className="text-sm font-medium text-gray-800 line-clamp-1">
                        {book.name}
                      </p>
                      <p className="text-xs text-gray-500">
                        Quantity: {book.quantity}
                      </p>
                    </div>
                    <button
                      onClick={() => removeFromCart(book.bookId)}
                      className="text-red-500 hover:text-red-700 text-xl"
                      aria-label={`Remove ${book.name} from cart`}
                    >
                      &times;
                    </button>
                  </div>
                ))}
              </div>

              <div className="p-4 border-t">
                <button
                  onClick={handleBorrowRequest}
                  disabled={isLoading}
                  className={`w-full ${
                    isLoading ? "bg-blue-400" : "bg-blue-600 hover:bg-blue-700"
                  } text-white text-sm font-medium py-2 rounded-xl flex items-center justify-center transition-colors`}
                  aria-busy={isLoading}
                >
                  {isLoading ? (
                    <>
                      <svg
                        className="animate-spin -ml-1 mr-2 h-4 w-4 text-white"
                        xmlns="http://www.w3.org/2000/svg"
                        fill="none"
                        viewBox="0 0 24 24"
                        aria-hidden="true"
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
                      Processing...
                    </>
                  ) : (
                    `Send Borrow Request (${totalItems}/5)`
                  )}
                </button>
              </div>
            </>
          ) : (
            <div className="p-4 text-center text-gray-500">
              Your cart is empty
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default CartDropdown;
