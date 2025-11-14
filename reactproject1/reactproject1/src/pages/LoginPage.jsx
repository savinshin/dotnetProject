import React, { useState } from "react";
import { useNavigate, useLocation, Link } from "react-router-dom";
import { useAuth } from "../auth/AuthContext.jsx";

export default function LoginPage() {
  const { login, loading } = useAuth();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();
  const location = useLocation();
  const from = location.state?.from?.pathname || "/";

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    const result = await login(email, password);
    if (!result.ok) {
      setError(result.error);
      return;
    }

    navigate(from, { replace: true });
  };

  return (
    <div className="hero min-h-[calc(100vh-4rem)] bg-base-200">
      <div className="hero-content flex-col">
        <div className="card w-full max-w-md bg-base-100 shadow-xl">
          <div className="card-body">
            <h2 className="card-title justify-center mb-2">Login</h2>
            <form onSubmit={handleSubmit} className="form-control gap-3">
              <label className="form-control">
                <span className="label-text">Email</span>
                <input
                  type="email"
                  className="input input-bordered"
                  autoComplete="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
              </label>

              <label className="form-control">
                <span className="label-text">Password</span>
                <input
                  type="password"
                  className="input input-bordered"
                  autoComplete="current-password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                />
              </label>

              {error && (
                <div className="alert alert-error py-2 mt-2 text-sm">
                  <span>{error}</span>
                </div>
              )}

              <div className="mt-3">
                <button
                  type="submit"
                  className="btn btn-primary w-full"
                  disabled={loading}
                >
                  {loading ? "Logging in..." : "Login"}
                </button>
              </div>
            </form>

            <p className="mt-4 text-sm text-center">
              No account?{" "}
              <Link to="/register" className="link link-primary">
                Register
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
