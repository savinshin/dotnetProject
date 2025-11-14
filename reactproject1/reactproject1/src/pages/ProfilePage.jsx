import React from "react";
import { useAuth } from "../auth/AuthContext.jsx";

export default function ProfilePage() {
  const { user } = useAuth();

  if (!user) return null;

  return (
    <div className="hero min-h-[calc(100vh-4rem)] bg-base-200">
      <div className="hero-content">
        <div className="card bg-base-100 shadow-xl w-full max-w-md">
          <div className="card-body">
            <h2 className="card-title mb-2">Profile</h2>
            <div className="space-y-2 text-sm">
              <div className="flex justify-between">
                <span className="font-semibold">Id</span>
                <span className="font-mono text-xs">{user.id}</span>
              </div>
              <div className="flex justify-between">
                <span className="font-semibold">Email</span>
                <span>{user.email}</span>
              </div>
              <div className="flex justify-between">
                <span className="font-semibold">Name</span>
                <span>{user.name}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
