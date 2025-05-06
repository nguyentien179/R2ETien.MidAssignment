import { useState, useEffect, useContext, createContext } from "react";
import { Link, useNavigate } from "react-router-dom";
import axios from "axios";
import CartDropdown from "./CartDropdown";
import Swal from "sweetalert2";

export const AuthContext = createContext();
export const CartContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const API_URL = import.meta.env.VITE_API_URL;

  // Configure axios instance for the provider
  const authAxios = axios.create({
    baseURL: API_URL,
    withCredentials: true,
  });

  let isRefreshing = false;
  let failedRequestsQueue = [];

  // Add response interceptor
  authAxios.interceptors.response.use(
    (response) => response,
    async (error) => {
      const originalRequest = error.config;

      if (
        error.response?.status === 401 &&
        !originalRequest.url.includes("/auth/") &&
        !originalRequest._retry
      ) {
        if (isRefreshing) {
          return new Promise((resolve, reject) => {
            failedRequestsQueue.push({ resolve, reject });
          })
            .then(() => authAxios(originalRequest))
            .catch((err) => Promise.reject(err));
        }

        originalRequest._retry = true;
        isRefreshing = true;

        try {
          await axios.post(
            `${API_URL}/users/refresh-token`,
            {},
            {
              withCredentials: true,
            }
          );
          return authAxios(originalRequest);
        } catch (refreshError) {
          handleLogout();
          return Promise.reject(refreshError);
        } finally {
          isRefreshing = false;
        }
      }
      return Promise.reject(error);
    }
  );

  const verifyAuth = async () => {
    try {
      const response = await authAxios.get("/users/current-user");
      setUser(response.data);
    } catch (error) {
      setUser(null);
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = async () => {
    try {
      await authAxios.post("/users/logout");
    } finally {
      setUser(null);
      navigate("/login");
    }
  };

  const updateUser = (userData) => {
    setUser(userData);
  };

  // Verify auth status on initial load
  useEffect(() => {
    verifyAuth();
  }, []);

  return (
    <AuthContext.Provider
      value={{
        user,
        loading,
        updateUser,
        logout: handleLogout,
        authAxios,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};

// Cart provider component
export const CartProvider = ({ children }) => {
  const [cartItems, setCartItems] = useState([]);

  const addToCart = (book) => {
    const totalQuantity = cartItems.reduce(
      (sum, item) => sum + item.quantity,
      0
    );
    if (totalQuantity >= 5) {
      alert("You can only borrow up to 5 books per request");
      return;
    }

    setCartItems((prev) => {
      const existing = prev.find((item) => item.bookId === book.bookId);
      if (existing) {
        return prev.map((item) =>
          item.bookId === book.bookId
            ? { ...item, quantity: item.quantity + 1 }
            : item
        );
      }
      return [...prev, { ...book, quantity: 1 }];
    });
  };

  const removeFromCart = (bookId) => {
    setCartItems((prev) =>
      prev
        .map((item) =>
          item.bookId === bookId
            ? { ...item, quantity: item.quantity - 1 }
            : item
        )
        .filter((item) => item.quantity > 0)
    );
  };

  const clearCart = () => {
    setCartItems([]);
  };

  return (
    <CartContext.Provider
      value={{ cartItems, addToCart, removeFromCart, clearCart }}
    >
      {children}
    </CartContext.Provider>
  );
};

// Header component
const Header = () => {
  const { user, logout } = useContext(AuthContext);
  const { cartItems, removeFromCart } = useContext(CartContext);
  const [isCartOpen, setIsCartOpen] = useState(false);
  const navigate = useNavigate();

  return (
    <header className="bg-white">
      <nav className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16">
          <div className="flex items-center">
            <Link to="/" className="text-xl font-bold text-gray-900">
              Library System
            </Link>
            <div className="hidden md:ml-6 md:flex md:space-x-8">
              <Link
                to="/books"
                className="text-gray-700 hover:text-gray-900 px-3 py-2"
              >
                Books
              </Link>
              <Link
                to="/about"
                className="text-gray-700 hover:text-gray-900 px-3 py-2"
              >
                About Us
              </Link>
            </div>
          </div>

          <div className="flex items-center space-x-4">
            <CartDropdown />

            {user ? (
              <div className="flex items-center space-x-4">
                <span className="text-gray-700">Welcome, {user.username}</span>
                <button
                  onClick={logout}
                  className="text-gray-700 hover:text-gray-900"
                >
                  Logout
                </button>
              </div>
            ) : (
              <div className="flex space-x-4">
                <Link to="/login" className="text-gray-700 hover:text-gray-900">
                  Login
                </Link>
                <Link
                  to="/register"
                  className="text-gray-700 hover:text-gray-900"
                >
                  Register
                </Link>
              </div>
            )}
          </div>
        </div>
      </nav>
    </header>
  );
};

export default Header;
