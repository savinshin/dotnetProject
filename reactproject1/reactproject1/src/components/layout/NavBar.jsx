import React from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../../auth/AuthContext.jsx";

export default function NavBar() {
  const { isAuthenticated, user, logout } = useAuth();

  return (
    <div className="navbar bg-base-100 shadow-md">
      <div className="flex-1">
        <Link to="/" className="btn btn-ghost normal-case text-xl">
          Aspire Auth
        </Link>
      </div>
      <div className="flex-none gap-2">
        {isAuthenticated && user ? (
          <>
            <span className="text-sm text-base-content/70 hidden sm:inline">
              {user.email}
            </span>
            <Link to="/me" className="btn btn-ghost btn-sm">
              Profile
            </Link>
            <button
              onClick={logout}
              className="btn btn-error btn-sm text-white"
            >
              Logout
            </button>
          </>
        ) : (
          <>
            <Link to="/login" className="btn btn-ghost btn-sm">
              Login
            </Link>
            <Link
              to="/register"
              className="btn btn-primary btn-sm text-white"
            >
              Register
            </Link>
          </>
        )}
      </div>
    </div>
  );
}
