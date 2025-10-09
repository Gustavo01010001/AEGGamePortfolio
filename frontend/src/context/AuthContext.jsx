import { createContext, useContext, useMemo, useState, useEffect } from "react";
import { login as apiLogin } from "../services/auth";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [token, setToken] = useState(localStorage.getItem("token"));
  const [role, setRole] = useState(localStorage.getItem("role"));
  const isAuthenticated = !!token;
  const isAdmin = role === "admin";

  const login = async ({ email, senha }) => {
    const res = await apiLogin({ email, senha });
    localStorage.setItem("token", res.token);
    localStorage.setItem("role", res.role || "user");
    setToken(res.token);
    setRole(res.role || "user");
  };

  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    setToken(null);
    setRole(null);
  };

  const value = useMemo(
    () => ({ token, role, isAuthenticated, isAdmin, login, logout }),
    [token, role, isAuthenticated, isAdmin]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export const useAuth = () => useContext(AuthContext);
