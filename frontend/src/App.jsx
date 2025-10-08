import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import Home from "./pages/Home";
import GameDetails from "./pages/GameDetails";

export default function App() {
  return (
    <BrowserRouter>
      <header style={{padding:16,borderBottom:"1px solid #333"}}>
        <Link to="/" style={{textDecoration:"none"}}>AEG • Portfólio</Link>
      </header>
      <main style={{padding:16}}>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/game/:id" element={<GameDetails />} />
        </Routes>
      </main>
    </BrowserRouter>
  );
}
