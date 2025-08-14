import { useEffect } from "react";
import { useDispatch } from "react-redux";
import { ActionTypes } from "../store/types";

const wsUrl = "wss://localhost:7114/ws/favorites"; 
//se agrega un web socket listener para recibir actualizaciones de favoritos
//para cada cosa que quiero escuchar? 

const FavoritesWebSocketListener = () => {
  const dispatch = useDispatch();

//   useEffect(() => {
//     const ws = new WebSocket(wsUrl);

//     ws.onopen = () => {
//       console.log("WebSocket conectado");
//     };

//     ws.onmessage = (event) => {
//         console.log("Mensaje recibido:");
//         console.log("Mensaje recibido:", event.data);
//       const [action, idStr, email] = event.data.split(":");
//       const id = parseInt(idStr, 10);
//       console.log(`Received message: ${event.data}`);
//       console.log(event.data.split(':'))

//       if (action === "added") {
//         console.log(`Favorite added: ${id} for ${email}`);
//       } else if (action === "deleted") {
//         console.log(`Favorite deleted: ${id} for ${email}`);
//       }
//     };

//     ws.onclose = () => {
//       console.log("WebSocket desconectado");
//     };

//     return () => {
//     //   ws.close();
//     };
//   }, [dispatch]);

  return null;
};

export default FavoritesWebSocketListener;