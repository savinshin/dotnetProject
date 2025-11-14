import React from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../auth/AuthContext.jsx";

export default function HomePage() {
  const { isAuthenticated, user } = useAuth();

  return (
    <div className="hero min-h-[calc(100vh-4rem)] bg-base-200">
      <div className="hero-content text-center">
        <div className="max-w-md">
          <h1 className="text-4xl font-bold mb-4">Welcome</h1>
          {isAuthenticated && user ? (
            <p className="mb-4">
              You are logged in as{" "}
              <span className="font-semibold">{user.email}</span>.
            </p>
          ) : (
            <p className="mb-4">
              Please{" "}
              <Link className="link link-primary" to="/login">
                log in
              </Link>{" "}
              or{" "}
              <Link className="link link-primary" to="/register">
                register
              </Link>
              .
            </p>
          )}
        </div>
      </div>
    </div>
  );
}
