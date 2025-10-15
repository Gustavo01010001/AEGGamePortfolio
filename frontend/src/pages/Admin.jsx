import { useEffect, useState } from "react";
import api, { imgUrl } from "../services/api";
import "./Admin.css";

export default function Admin() {
  const [games, setGames] = useState([]);
  const [form, setForm] = useState({
    id: null,
    titulo: "",
    genero: "",
    ano: "",
    status: "",
    imagemUrl: "",
    linkDemo: "",
  });

  const [editando, setEditando] = useState(false);

  // 🔹 Busca os jogos no backend
  async function carregarGames() {
    try {
      const res = await api.get("/api/games");
      setGames(res.data);
    } catch (err) {
      console.error("Erro ao carregar jogos:", err);
    }
  }

  useEffect(() => {
    carregarGames();
  }, []);

  // 🔹 Atualiza o formulário conforme digita
  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  // 🔹 Cadastrar novo jogo
  async function handleSubmit(e) {
    e.preventDefault();

    try {
      if (editando) {
        await api.put(`/api/games/${form.id}`, form);
      } else {
        await api.post("/api/games", form);
      }

      setForm({
        id: null,
        titulo: "",
        genero: "",
        ano: "",
        status: "",
        imagemUrl: "",
        linkDemo: "",
      });
      setEditando(false);
      carregarGames();
    } catch (error) {
      console.error("Erro ao salvar jogo:", error);
    }
  }

  // 🔹 Editar jogo
  function handleEdit(game) {
    setForm(game);
    setEditando(true);
  }

  // 🔹 Excluir jogo
  async function handleDelete(id) {
    if (confirm("Deseja excluir este jogo?")) {
      await api.delete(`/api/games/${id}`);
      carregarGames();
    }
  }

  return (
    <div className="admin-container">
      <h1>🎮 Painel Administrativo</h1>

      {/* 📋 Formulário */}
      <form onSubmit={handleSubmit} className="admin-form">
        <input
          type="text"
          name="titulo"
          placeholder="Título"
          value={form.titulo}
          onChange={handleChange}
          required
        />
        <input
          type="text"
          name="genero"
          placeholder="Gênero"
          value={form.genero}
          onChange={handleChange}
          required
        />
        <input
          type="number"
          name="ano"
          placeholder="Ano"
          value={form.ano}
          onChange={handleChange}
          required
        />
        <input
          type="text"
          name="status"
          placeholder="Status (Lançado, Em breve...)"
          value={form.status}
          onChange={handleChange}
        />
        <input
          type="text"
          name="imagemUrl"
          placeholder="URL da imagem"
          value={form.imagemUrl}
          onChange={handleChange}
        />
        <input
          type="text"
          name="linkDemo"
          placeholder="Link da demo"
          value={form.linkDemo}
          onChange={handleChange}
        />
        <button type="submit">
          {editando ? "Atualizar Jogo" : "Adicionar Jogo"}
        </button>
      </form>

      {/* 🕹️ Lista de jogos */}
      <div className="games-grid">
        {games.map((g) => (
          <div key={g.id} className="game-card">
            <img
              src={imgUrl(g.imagemUrl || "")}
              alt={g.titulo}
              onError={(e) => (e.target.src = "/no-image.png")}
            />
            <h3>{g.titulo}</h3>
            <p>{g.genero} • {g.ano}</p>
            <p><strong>Status:</strong> {g.status}</p>
            <div className="admin-actions">
              <button onClick={() => handleEdit(g)}>✏️ Editar</button>
              <button onClick={() => handleDelete(g.id)}>🗑️ Excluir</button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
