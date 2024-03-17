import { useEffect, useRef, useState } from "react";

export default function Room() {
  const [loggedIn, setLoggedIn] = useState(false);
  const [chats, setChats] = useState([]);
  const [socket, setSocket] = useState(null);

  const [message, setMessage] = useState("");
  const username = "User One";

  var nextId = useRef(1);

  useEffect(() => {
    const ws = new WebSocket("ws://localhost:8083");

    ws.onopen = () => {
      console.log("Connected to ws");
      setSocket(ws);
    };

    ws.onmessage = (event) => {
      try {
        const serverMessage = JSON.parse(event.data);

        setChats((prevChats) => [
          ...prevChats,
          {
            id: nextId.current++,
            username: serverMessage.username,
            value: serverMessage.message,
          },
        ]);
      } catch (error) {
        console.error("Error parsing message:", error);
      }
    };

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

  useEffect(() => {
    console.log("Updated chats:", chats);
  }, [chats]);

  function handleLogin() {
    setLoggedIn(true);
  }

  function handleSendMessage() {
    if (socket && loggedIn) {
      const payload = {
        Type: "CHAT",
        Username: username,
        Message: message,
      };

      socket.send(JSON.stringify(payload));
    }
  }

  return (
    <div>
      {loggedIn ? (
        <div>
          {socket ? (
            <div>
              <h1>Hello</h1>
              <ul>
                {chats.map((chat, index) => (
                  <li key={index}>
                    {chat.username}: {chat.value}
                  </li>
                ))}
              </ul>

              <input
                type="text"
                value={message}
                onChange={(e) => setMessage(e.target.value)}
                placeholder="Type your message..."
              />
              <button onClick={() => handleSendMessage()}>Send</button>
            </div>
          ) : (
            <div>
              <p>Unable to establish connection to socket</p>
            </div>
          )}
        </div>
      ) : (
        <div>
          <p>You are not authorized</p>
          <button onClick={handleLogin}>Log In</button>
        </div>
      )}
    </div>
  );
}
