import AdminSideBar from "./components/AdminSIdeBar";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import AdminBooksPage from "./pages/Admin/Book/AdminBookPage";
import AdminAddBookPage from "./pages/Admin/Book/AdminAddBookPage";
import AdminUpdateBookPage from "./pages/Admin/Book/AdminUpdateBookPage";

function App() {
  return (
    <>
      <BrowserRouter>
        <div className="flex">
          <AdminSideBar />
          <div className="flex-1 p-4">
            <Routes>
              <Route path="/admin/dashboard" element={<AdminBooksPage />} />
              <Route path="/admin/books" element={<AdminBooksPage />} />
              <Route path="/admin/books/add" element={<AdminAddBookPage />} />
              <Route
                path="/admin/books/:id/update"
                element={<AdminUpdateBookPage />}
              />
              <Route path="/admin/categories" element={<AdminBooksPage />} />
              <Route
                path="/admin/borrowing-request"
                element={<AdminBooksPage />}
              />
              <Route path="/admin/users" element={<AdminBooksPage />} />
            </Routes>
          </div>
        </div>
      </BrowserRouter>
    </>
  );
}

export default App;
