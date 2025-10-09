import api from "./api";

export const getGames = async () => (await api.get("/api/Games")).data;
export const getGame  = async (id) => (await api.get(`/api/Games/${id}`)).data;

export const createGame = async (game) =>
  (await api.post("/api/Games", game)).data;

export const updateGame = async (id, game) =>
  (await api.put(`/api/Games/${id}`, game)).data;

export const deleteGame = async (id) =>
  (await api.delete(`/api/Games/${id}`)).data;
