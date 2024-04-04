import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

export default function ChatRoom() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [chats, setChats] = useState([]);
  const [chatMode, setChatMode] = useState(false);
  var nextId = 0;

  const [socket, setSocket] = useState(null);

  const [userInput, setUserInput] = useState("");

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
        setLoading(false);
      });
  }, []);

  useEffect(() => {
    if (chatMode) {
      const webSocket = new WebSocket(`wss://localhost:7025/ws?room=${room}`);
      setSocket(webSocket);

      webSocket.onopen = function (event) {
        console.log("WebSocket connection established.");
      };

      webSocket.onmessage = function (event) {
        try {
          const serverMessages = JSON.parse(event.data);
          console.log("Ser: " + serverMessages);

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

  while (loading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error}</div>;
  }

  async function sendMessage() {
    if (socket) {
      socket.send(userInput);
    }

    setUserInput("");
  }

  return (
    <div>
      <h1>Chat</h1>
      {chats.map((chat) => (
        <li key={chat.key}>{chat.value}</li>
      ))}

      <br />

      <input
        type="text"
        placeholder="Type your message"
        value={userInput}
        onChange={(e) => setUserInput(e.target.value)}
      />
      <button
        onClick={() => {
          sendMessage();
        }}
      >
        Send Message
      </button>
    </div>
  );
}
