import { createContext, useContext, useState, useEffect } from "react";
import api from "../services/api";

const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isAdmin, setIsAdmin] = useState(false);

  useEffect(() => {
    const token = localStorage.getItem("token");
    const role = localStorage.getItem("role");

    if (token && role) {
      setIsAuthenticated(true);
      setIsAdmin(role === "admin");
      setUser({ role });
    }
  }, []);

  async function login(email, senha) {
    try {
      const response = await api.post("/api/Auth/login", {
        Email: email,
        Senha: senha,
      });

      const { token, role } = response.data;

      localStorage.setItem("token", token);
      localStorage.setItem("role", role);

      setUser({ email, role });
      setIsAuthenticated(true);
      setIsAdmin(role === "admin");
    } catch (err) {
      console.error("Erro no login:", err.response?.data || err.message);
      throw new Error("Usuário ou senha incorretos.");
    }
  }

  function logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    setUser(null);
    setIsAuthenticated(false);
    setIsAdmin(false);
  }

  return (
    <AuthContext.Provider
      value={{
        user,
        login,
        logout,
        isAuthenticated,
        isAdmin,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
