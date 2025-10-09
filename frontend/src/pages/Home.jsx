import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import api, { imgUrl } from "../services/api";

export default function Home() {
  const [games, setGames] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    api.get("/Games")
      .then(r => setGames(r.data))
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <p>Carregando...</p>;
  if (!games.length) return <p>Sem jogos cadastrados ainda.</p>;

  return (
    <div style={{display:"grid",gap:16,gridTemplateColumns:"repeat(auto-fill,minmax(240px,1fr))"}}>
      {games.map(g => (
        <Link key={g.id} to={`/game/${g.id}`} style={{textDecoration:"none",color:"inherit"}}>
          <div style={{border:"1px solid #444",borderRadius:12,overflow:"hidden"}}>
            {g.imagemUrl && <img src={g.imagemUrl} alt={g.titulo} style={{width:"100%",height:140,objectFit:"cover"}}/>}
            <div style={{padding:12}}>
              <h3 style={{margin:0}}>{g.titulo}</h3>
              <small>{g.genero} • {g.ano} • {g.status}</small>
            </div>
          </div>
        </Link>
      ))}
    </div>
  );
}
