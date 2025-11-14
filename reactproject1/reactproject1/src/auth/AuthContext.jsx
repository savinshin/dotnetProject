import React, { createContext, useContext, useEffect, useState } from "react";
import { api } from "../api/client";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [token, setToken] = useState(() => localStorage.getItem("auth_token"));
  const [user, setUser] = useState(() => {
    const raw = localStorage.getItem("auth_user");
    if (!raw) return null;
    try {
      return JSON.parse(raw);
    } catch {
      return null;
    }
  });
  const [loading, setLoading] = useState(false);
  const [initialized, setInitialized] = useState(false);

  useEffect(() => {
    if (!token) {
      setInitialized(true);
      return;
    }

    (async () => {
      try {
        const me = await api.me();
        setUser(me);
        localStorage.setItem("auth_user", JSON.stringify(me));
      } catch (err) {
        console.warn("Failed to load current user:", err);
        setToken(null);
        setUser(null);
        localStorage.removeItem("auth_token");
        localStorage.removeItem("auth_user");
      } finally {
        setInitialized(true);
      }
    })();
  }, [token]);

  const login = async (email, password) => {
    setLoading(true);
    try {
      const loginRes = await api.login({ email, password });
      const accessToken = loginRes.accessToken;

      setToken(accessToken);
      localStorage.setItem("auth_token", accessToken);

      const me = await api.me();
      setUser(me);
      localStorage.setItem("auth_user", JSON.stringify(me));

      return { ok: true };
    } catch (err) {
      console.error("Login error:", err);
      return { ok: false, error: err.message };
    } finally {
      setLoading(false);
    }
  };

  const register = async (email, password, userName) => {
    setLoading(true);
    try {
      await api.register({ email, password, userName });
      return { ok: true };
    } catch (err) {
      console.error("Register error:", err);
      return { ok: false, error: err.message };
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    setToken(null);
    setUser(null);
    localStorage.removeItem("auth_token");
    localStorage.removeItem("auth_user");
  };

  const value = {
    token,
    user,
    isAuthenticated: !!user,
    loading,
    initialized,
    login,
    register,
    logout,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
}
