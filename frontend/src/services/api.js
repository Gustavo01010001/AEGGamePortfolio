import axios from "axios";

export const baseURL = "http://localhost:5000";

export const imgUrl = `${baseURL}/uploads/`;


const api = axios.create({
  baseURL: baseURL,
  headers: { "Content-Type": "application/json" },
});

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
  },
  (error) => Promise.reject(error)
);


export default api;
