import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

export default function ChatRoom() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [chats, setChats] = useState([]);
  const [chatMode, setChatMode] = useState(false);
  var nextId = 0;

  const { room } = useParams();

  useEffect(() => {
    fetch(`https://localhost:7025/api/rooms/enter?roomName=${room}`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      credentials: "include",
    })
      .then(async (response) => {
        if (response.ok) {
          setChatMode(true);
          setLoading(false);
        } else {
          setError("Failed to enter the room. Status: " + response.status);
          setLoading(false);
        }
      })
      .catch((error) => {
        setError("An error occurred while entering the room: " + error.message);
      });
  }, []);

  useEffect(() => {
    if (chatMode) {
      const webSocket = new WebSocket(`wss://localhost:7025/ws?room=${room}`);

      webSocket.onopen = function (event) {
        console.log("WebSocket connection established.");
      };

      webSocket.onmessage = function (event) {
        try {
          const serverMessages = JSON.parse(event.data);

          serverMessages.forEach((msg) => {
            setChats((prevChats) => [
              ...prevChats,
              {
                key: nextId++,
                value: msg,
              },
            ]);
          });
        } catch (error) {
          console.error("Error parsing message:", error);
        }
      };

      webSocket.onclose = function (event) {
        console.log("WebSocket connection closed.");
      };

      webSocket.onerror = function (event) {
        console.error("WebSocket error:", event);
      };

      return () => {
        webSocket.close();
      };
    }
  }, [chatMode]);

  if (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error}</div>;
  }

  return (
    <div>
      <h1>Chat</h1>
      {chats.map((chat) => (
        <li key={chat.key}>{chat.value}</li>
      ))}
    </div>
  );
}
