import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import api, { imgUrl } from "../services/api";
import "./Home.css";

export default function Home() {
  const [games, setGames] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchGames = async () => {
      try {
        const response = await api.get("/api/games");
        setGames(response.data);
      } catch (error) {
        console.error("Erro ao carregar jogos:", error);
      } finally {
        setLoading(false);
      }
    };
    fetchGames();
  }, []);

  if (loading) return <p>Carregando catálogo...</p>;

  return (
    <div className="home-container">
      <h1>🎮 Catálogo de Jogos AEG Studio</h1>
      <div className="games-grid">
        {games.map((game) => (
          <div key={game.id} className="game-card">
            <img
              src={imgUrl(game.imagemUrl || "")}
              alt={game.titulo}
              className="game-img"
              onError={(e) => (e.target.src = "/no-image.png")}
            />
            <h2>{game.titulo}</h2>
            <p>Gênero: {game.genero}</p>
            <p>Ano: {game.ano}</p>
            <Link to={`/game/${game.id}`} className="btn">
              Ver mais
            </Link>
          </div>
        ))}
      </div>
    </div>
  );
}
