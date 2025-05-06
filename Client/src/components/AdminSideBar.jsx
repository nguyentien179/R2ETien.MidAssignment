import React from "react";
import { NavLink } from "react-router-dom";
const navItems = [
  { name: "Dashboard", href: "/admin/dashboard" },
  { name: "Books", href: "/admin/books" },
  { name: "Categories", href: "/admin/categories" },
  {
    name: "Borrowing Request",
    href: "/admin/borrowing-request",
  },
  { name: "Users", href: "/admin/users" },
];
export default function AdminSideBar() {
  return (
    <div className="h-screen w-64 bg-gray-100 border-r shadow-sm flex flex-col">
      <div className="bg-blue-600 text-white text-xl font-bold p-4 text-center">
        Library System
      </div>
      <nav className="flex-1 p-4 space-y-2">
        {navItems.map((item) => (
          <NavLink
            key={item.name}
            to={item.href}
            className={({ isActive }) =>
              `flex items-center gap-3 text-gray-700 hover:bg-blue-100 p-2 rounded-lg transition-colors ${
                isActive ? "bg-blue-200 text-blue-700 font-semibold" : ""
              }`
            }
          >
            {item.icon}
            <span>{item.name}</span>
          </NavLink>
        ))}
      </nav>
    </div>
  );
}
