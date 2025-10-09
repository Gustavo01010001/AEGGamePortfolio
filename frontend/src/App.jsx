import { BrowserRouter, Routes, Route } from "react-router-dom";
import { AuthProvider, useAuth } from "./context/AuthContext";
import Home from "./pages/Home";
import GameDetails from "./pages/GameDetails";
import Login from "./pages/Login";
import Admin from "./pages/Admin";
import PrivateRoute from "./components/PrivateRoute";
import "./App.css";

function TopBar() {
  const { isAuthenticated, isAdmin, logout } = useAuth();
  return (
    <div style={{ padding: 12, display: "flex", gap: 12 }}>
      <a href="/">Home</a>
      {isAdmin && <a href="/admin">Admin</a>}
      {isAuthenticated ? (
        <button onClick={logout}>Sair</button>
      ) : (
        <a href="/login">Entrar</a>
      )}
    </div>
  );
}

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <TopBar />
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/game/:id" element={<GameDetails />} />
          <Route path="/login" element={<Login />} />
          <Route
            path="/admin"
            element={
              <PrivateRoute adminOnly>
                <Admin />
              </PrivateRoute>
            }
          />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

/* Exemplo de usuário admin para login (criado diretamente no banco de dados MySQL):
{
  "nome": "Gusta",
  "email": "gusta@aeg.com",
  "senha": "12345"
}*/