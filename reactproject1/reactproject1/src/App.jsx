import React from "react";
import { Routes, Route } from "react-router-dom";
import NavBar from "./components/layout/NavBar.jsx";
import HomePage from "./pages/HomePage.jsx";
import LoginPage from "./pages/LoginPage.jsx";
import RegisterPage from "./pages/RegisterPage.jsx";
import ProfilePage from "./pages/ProfilePage.jsx";
import { AuthGuard } from "./components/AuthGuard.jsx";

export default function App() {
  return (
    <div className="min-h-screen bg-base-200">
      <NavBar />
      <main className="px-2">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route
            path="/me"
            element={
              <AuthGuard>
                <ProfilePage />
              </AuthGuard>
            }
          />
        </Routes>
      </main>
    </div>
  );
}
