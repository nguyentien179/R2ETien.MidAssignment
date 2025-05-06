import AdminSideBar from "./components/AdminSIdeBar";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import AdminBooksPage from "./pages/Admin/Book/AdminBookPage";
import AdminAddBookPage from "./pages/Admin/Book/AdminAddBookPage";
import AdminUpdateBookPage from "./pages/Admin/Book/AdminUpdateBookPage";
import Header, {
  AuthProvider,
  CartProvider,
  useAuth,
} from "./components/Header";
import AdminCategoryPage from "./pages/Admin/Category/AdminCategoryPage";
import AdminAddCategoryPage from "./pages/Admin/Category/AdminAddCategoryPage";
import AdminUpdateCategoryPage from "./pages/Admin/Category/AdminUpdateCategoryPage";
import AdminBookBorrowingRequestPage from "./pages/Admin/BorrowRequest/AdminBorrowRequest";
import AdminUserPage from "./pages/Admin/User/AdminUserPage";
import HomePage from "./pages/Client/HomepPage";
import RegisterPage from "./pages/Client/Register";
import LoginPage from "./pages/Client/Login";
import BooksPage from "./pages/Client/BookPage";
import AboutUsPage from "./pages/Client/AboutUsPage";
import BookDetailsPage from "./pages/Client/BookDetailPage";

const ProtectedAdminRoute = ({ children }) => {
  const { user, loading } = useAuth();

  if (loading) {
    return (
      <div className="flex justify-center items-center h-screen">
        Loading...
      </div>
    );
  }

  if (!user || user.role !== 1) {
    return <Navigate to="/login" replace />;
  }

  return children;
};

function App() {
  const isAdminRoute = location.pathname.startsWith("/admin");
  return (
    <>
      <BrowserRouter>
        <AuthProvider>
          <CartProvider>
            {!isAdminRoute && <Header />}
            <Routes>
              {/* Public Routes */}
              <Route path="/" element={<HomePage />} />
              <Route path="/login" element={<LoginPage />} />
              <Route path="/register" element={<RegisterPage />} />
              <Route path="/books" element={<BooksPage />} />
              <Route path="/about" element={<AboutUsPage />} />
              <Route path="/book/:id" element={<BookDetailsPage />} />

              {/* Admin Routes */}
              <Route
                path="/admin/*"
                element={
                  <ProtectedAdminRoute>
                    <div className="flex min-h-screen">
                      <AdminSideBar />
                      <div className="flex-1 p-4">
                        <Routes>
                          <Route
                            path="/dashboard"
                            element={<AdminBooksPage />}
                          />
                          <Route path="books" element={<AdminBooksPage />} />
                          <Route
                            path="/books/add"
                            element={<AdminAddBookPage />}
                          />
                          <Route
                            path="/books/:id/update"
                            element={<AdminUpdateBookPage />}
                          />
                          <Route
                            path="/categories"
                            element={<AdminCategoryPage />}
                          />
                          <Route
                            path="/categories/add"
                            element={<AdminAddCategoryPage />}
                          />
                          <Route
                            path="/categories/:id/update"
                            element={<AdminUpdateCategoryPage />}
                          />
                          <Route
                            path="/borrowing-request"
                            element={<AdminBookBorrowingRequestPage />}
                          />
                          <Route path="/users" element={<AdminUserPage />} />
                        </Routes>
                      </div>
                    </div>
                  </ProtectedAdminRoute>
                }
              />
            </Routes>
          </CartProvider>
        </AuthProvider>
      </BrowserRouter>
    </>
  );
}

export default App;
