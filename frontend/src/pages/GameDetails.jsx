import { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import api, { imgUrl } from "../services/api";

export default function GameDetails() {
  const { id } = useParams();
  const [game, setGame] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    api.get(`/Games/${id}`)
      .then(r => setGame(r.data))
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [id]);

  if (loading) return <p>Carregando...</p>;
  if (!game) return <p>Jogo não encontrado. <Link to="/">Voltar</Link></p>;

  return (
    <div style={{maxWidth:900,margin:"0 auto"}}>
      {game.imagemUrl && <img src={game.imagemUrl} alt={game.titulo} style={{width:"100%",borderRadius:12}}/>}
      <h1>{game.titulo}</h1>
      <p><b>Gênero:</b> {game.genero}</p>
      <p><b>Ano:</b> {game.ano}</p>
      <p><b>Status:</b> {game.status}</p>
      {game.demoUrl && <p><a href={game.demoUrl} target="_blank" rel="noreferrer">Jogar demo</a></p>}
      <p><Link to="/">← Voltar</Link></p>
    </div>
  );
}
