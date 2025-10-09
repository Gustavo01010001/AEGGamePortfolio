import api from "./api";

export const login = async ({ email, senha }) =>
  (await api.post("/api/Auth/login", { email, senha })).data;

export const registerUser = async (payload) =>
  (await api.post("/api/Auth/register", payload)).data;
