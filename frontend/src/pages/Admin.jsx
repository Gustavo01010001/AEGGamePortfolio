import { useState } from "react";
import { createGame } from "../services/games";

const initial = {
  titulo: "",
  genero: "",
  ano: new Date().getFullYear(),
  status: "Em desenvolvimento",
  imagemUrl: "",   // ex: "images/Cuphead_capa.png"
  demoUrl: ""
};

export default function Admin() {
  const [form, setForm] = useState(initial);
  const [msg, setMsg] = useState("");

  const onSubmit = async (e) => {
    e.preventDefault();
    setMsg("");
    try {
      await createGame(form);
      setForm(initial);
      setMsg("Game criado!");
    } catch (e) {
      setMsg("Erro ao salvar.");
      console.error(e);
    }
  };

  return (
    <div style={{ padding: 16, color: "#fff", maxWidth: 520 }}>
      <h2>Painel • Novo jogo</h2>
      <form onSubmit={onSubmit} style={{ display: "grid", gap: 12 }}>
        {[
          ["titulo", "Título"],
          ["genero", "Gênero"],
          ["status", "Status"],
          ["imagemUrl", "Imagem (ex: images/Cuphead_capa.png)"],
          ["demoUrl", "URL da demo (opcional)"],
        ].map(([key, label]) => (
          <label key={key}>
            {label}
            <input
              value={form[key]}
              onChange={(e) => setForm({ ...form, [key]: e.target.value })}
            />
          </label>
        ))}
        <label>
          Ano
          <input
            type="number"
            value={form.ano}
            onChange={(e) => setForm({ ...form, ano: Number(e.target.value) })}
          />
        </label>
        <button type="submit">Salvar</button>
        {msg && <small>{msg}</small>}
      </form>
    </div>
  );
}
