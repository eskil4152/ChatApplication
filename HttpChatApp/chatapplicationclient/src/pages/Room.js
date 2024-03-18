import { useEffect, useState } from "react";

export default function Room() {
  const [chats, setChats] = useState([]);
  const [socket, setSocket] = useState(null);

  const [message, setMessage] = useState("");

  const [username, setUsername] = useState("");
  const [usernameTmp, setUsernameTmp] = useState("");

  let nextId = 0;

  useEffect(() => {
    const ws = new WebSocket("ws://192.168.0.135:8083");

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
            id: nextId++,
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

  function handleSendMessage() {
    if (socket && message && !(message === "")) {
      const payload = {
        Type: "CHAT",
        Username: username,
        Message: message,
      };

      socket.send(JSON.stringify(payload));

      setMessage("");
    } else {
      console.log("ENTER SOMETHING");
    }
  }

  return (
    <div>
      {!(username.length <= 1) ? (
        <div>
          {socket ? (
            <div>
              <h1>Hello</h1>
              <div>
                {chats.map((chat) => (
                  <p key={chat.id}>
                    {chat.username}: {chat.value}
                  </p>
                ))}
              </div>

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
          <p>Set username</p>

          <input
            value={usernameTmp}
            onChange={(e) => setUsernameTmp(e.target.value)}
          ></input>
          <button
            onClick={() => {
              setUsername(usernameTmp);
            }}
          >
            Confirm
          </button>
        </div>
      )}
    </div>
  );
}
