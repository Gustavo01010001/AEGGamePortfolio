import { useState } from "react";
import api from "../services/api";

export default function Login() {
  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");

  async function handleLogin(e) {
    e.preventDefault();
    try {
      const res = await api.post("/api/Auth/login", { email, senha });
      alert("Login realizado!");
      console.log(res.data);
      localStorage.setItem("token", res.data.token);
    } catch (err) {
      alert("Falha no login. Verifique suas credenciais.");
    }
  }

  return (
    <div style={{ textAlign: "center", marginTop: "80px" }}>
      <h2>Login</h2>
      <form onSubmit={handleLogin}>
        <input
          type="email"
          placeholder="E-mail"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />
        <br />
        <input
          type="password"
          placeholder="Senha"
          value={senha}
          onChange={(e) => setSenha(e.target.value)}
        />
        <br />
        <button type="submit">Entrar</button>
      </form>
    </div>
  );
}
