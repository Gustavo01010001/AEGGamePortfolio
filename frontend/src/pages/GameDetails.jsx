import { useParams, Link } from "react-router-dom";
import { useEffect, useState } from "react";
import api, { imgUrl } from "../services/api";
import "./GameDetails.css";

export default function GameDetails() {
  const { id } = useParams();
  const [game, setGame] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchGame() {
      try {
        const res = await api.get(`/api/games/${id}`);
        setGame(res.data);
      } catch (err) {
        console.error("Erro ao carregar o jogo:", err);
      } finally {
        setLoading(false);
      }
    }

    fetchGame();
  }, [id]);

  if (loading) return <p>Carregando...</p>;

  if (!game) {
    return (
      <div style={{ textAlign: "center", marginTop: "50px" }}>
        <p>Jogo não encontrado.</p>
        <Link to="/">Voltar</Link>
      </div>
    );
  }

  return (
    <div className="details-container">
      <div className="details-card">
        <img src={imgUrl(game.imagemUrl)} alt={game.titulo} />
        <h2>{game.titulo}</h2>

        <p><strong>Gênero:</strong> {game.genero}</p>
        <p><strong>Ano:</strong> {game.ano}</p>
        <p><strong>Status:</strong> {game.status}</p>

        {game.linkDemo ? (
          <a
            href={game.linkDemo}
            target="_blank"
            rel="noopener noreferrer"
            className="btn-demo"
          >
            🎮 Baixar Demo
          </a>
        ) : (
          <p style={{ color: "#aaa" }}>Nenhuma demo disponível.</p>
        )}

        <Link to="/" className="voltar-link">
          ← Voltar
        </Link>
      </div>
    </div>
  );
}
