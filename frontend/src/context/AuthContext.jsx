import { createContext, useContext, useState } from "react";
import api from "../services/api"; // usa o axios já configurado

const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);

 async function login(email, senha) {
  try {
    // enviar com as chaves capitalizadas para bater com LoginDto (Email, Senha)
    const response = await api.post("/api/Auth/login", { Email: email, Senha: senha });
    console.log("Resposta do backend:", response.data);
    const { token, role } = response.data;

    localStorage.setItem("token", token);
    localStorage.setItem("role", role);
    setUser({ email, role });
  } catch (err) {
    console.error("Erro no login:", err.response?.data || err.message);
    throw new Error("Usuário ou senha incorretos.");
  }
}


  function logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    setUser(null);
  }

  const isAuthenticated = !!localStorage.getItem("token");
  const isAdmin = localStorage.getItem("role") === "admin";

  return (
    <AuthContext.Provider value={{ user, login, logout, isAuthenticated, isAdmin }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
