import { useState, useEffect, useContext, createContext } from "react";
import { Link, useNavigate } from "react-router-dom";
import axios from "axios";
import CartDropdown from "./CartDropdown";

export const AuthContext = createContext();
export const CartContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const API_URL = import.meta.env.VITE_API_URL;

  // Verify auth status on initial load
  useEffect(() => {
    const verifyAuth = async () => {
      try {
        const response = await axios.get(`${API_URL}/users/current-user`, {
          withCredentials: true,
        });
        setUser(response.data);
      } catch (error) {
        setUser(null);
      } finally {
        setLoading(false);
      }
    };
    verifyAuth();
  }, [API_URL]);

  const updateUser = (userData) => {
    setUser(userData);
  };

  const logout = async () => {
    try {
      await axios.post(
        `${API_URL}/users/logout`,
        {},
        {
          withCredentials: true,
        }
      );
    } finally {
      setUser(null);
      navigate("/login");
    }
  };

  return (
    <AuthContext.Provider value={{ user, loading, updateUser, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  return useContext(AuthContext);
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
