import { useEffect, useState } from "react";
import { useWebSocket } from "../WebSocketContext";

export default function LoginPage() {
  const socket = useWebSocket();

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [loginType, setLoginType] = useState(false);

  const [message, setMessage] = useState("");

  useEffect(() => {
    if (!socket) return;

    function handleSocketMessage(e) {
      const message = JSON.parse(e.data);

      if (message.StatusCode === 200) {
        console.log("Ok");
        setMessage("OK");
      } else {
        console.log("Not ok");
        setMessage("Not ok");
      }
    }

    socket.addEventListener("message", handleSocketMessage);

    return () => {
      socket.removeEventListener("message", handleSocketMessage);
    };
  }, [socket]);

  function handleSubmit(e) {
    e.preventDefault();
    var loginTypeString = loginType ? "LOGIN" : "REGISTER";

    if (socket) {
      socket.send(
        JSON.stringify({
          Type: "LOGIN",
          LoginType: loginTypeString,
          Username: username,
          Password: password,
        })
      );
    }
  }

  return (
    <div>
      <form onSubmit={handleSubmit}>
        <input
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          placeholder="Username"
        />
        <input
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          placeholder="Password"
        />
        <button>Confirm</button>
      </form>

      <p>Currently {loginType ? "logging in" : "registering"}</p>
      <button
        onClick={() => {
          setLoginType(!loginType);
        }}
      >
        Switch between log in / register
      </button>

      <p>{message}</p>
    </div>
  );
}
