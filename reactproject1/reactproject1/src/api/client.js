const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "https://localhost:7194";

function getAuthToken() {
  return localStorage.getItem("auth_token");
}

async function request(path, options = {}) {
  const headers = new Headers(options.headers || {});

  if (!headers.has("Content-Type") && options.body) {
    headers.set("Content-Type", "application/json");
  }

  const token = getAuthToken();
  if (token) {
    headers.set("Authorization", `Bearer ${token}`);
  }

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers,
  });

  const text = await response.text();
  let data = null;
  try {
    data = text ? JSON.parse(text) : null;
  } catch {
    data = text;
  }

  if (!response.ok) {
    const errors =
      data && typeof data === "object"
        ? data.errors ??
          data.error ??
          data.title ??
          `Request failed with status ${response.status}`
        : `Request failed with status ${response.status}`;

    throw new Error(
      Array.isArray(errors) ? errors.join(", ") : String(errors)
    );
  }

  return data;
}

// { success: bool, data: ..., errors: [] }
function ensureSuccess(result) {
  if (!result) {
    throw new Error("Empty response from server");
  }
  if (!result.success) {
    throw new Error(
      result.errors && result.errors.length
        ? result.errors.join(", ")
        : "Server returned failure response"
    );
  }
  return result.data;
}

export const api = {
  async register({ email, password, userName }) {
    const res = await request("/api/auth/register", {
      method: "POST",
      body: JSON.stringify({ email, password, userName }),
    });
    return ensureSuccess(res);
  },

  async login({ email, password }) {
    const res = await request("/api/auth/login", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    });
    return ensureSuccess(res); // LoginResponse
  },

  async me() {
    const res = await request("/api/auth/me", {
      method: "GET",
    });
    return ensureSuccess(res); // CurrentUserResponse
  },
};
