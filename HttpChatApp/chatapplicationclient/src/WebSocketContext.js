import React, { createContext, useContext, useEffect, useState } from "react";

const WebSocketContext = createContext();

export const useWebSocket = () => useContext(WebSocketContext);

export const WebSocketProvider = ({ children }) => {
  const [socket, setSocket] = useState(null);

  useEffect(() => {
    const ws = new WebSocket("ws://192.168.0.135:8083");

    ws.onopen = () => {
      console.log("Connected to ws");
      setSocket(ws);
    };

    /*ws.onmessage = (event) => {
      try {
        const serverMessage = JSON.parse(event.data);

        setChats((prevChats) => [
          ...prevChats,
          {
            id: nextId++,
            username: serverMessage.username,
            value: serverMessage.message,
          },
        ]);
      } catch (error) {
        console.error("Error parsing message:", error);
      }
    };*/

    ws.onclose = () => {
      console.log("Disconnected from ws");
    };

    ws.onerror = (error) => {
      console.error("WebSocket error:", error);
      setSocket(null);
    };

    return () => {
      if (ws) {
        ws.close();
      }
    };
  }, []);

  return (
    <WebSocketContext.Provider value={socket}>
      {children}
    </WebSocketContext.Provider>
  );
};
