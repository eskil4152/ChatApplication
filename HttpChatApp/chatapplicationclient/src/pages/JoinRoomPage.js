import { useEffect, useState } from "react";
import { useWebSocket } from "../WebSocketContext";

export default function JoinRoomPage() {
  const socket = useWebSocket();

  const [roomName, setRoomName] = useState("");
  const [roomPassword, setRoomPassword] = useState("");

  useEffect(() => {
    if (!socket) return;

    function handleSocketMessage(e) {
      const message = JSON.parse(e.data);

      if (message.StatusCode === 200) {
        console.log("Ok");
      } else {
        console.log("Not ok");
      }
    }

    socket.addEventListener("message", handleSocketMessage);

    return () => {
      socket.removeEventListener("message", handleSocketMessage);
    };
  }, [socket]);

  function handleSubmit(e) {
    e.preventDefault();

    if (socket) {
      socket.send(
        JSON.stringify({
          Type: "JOINROOM",
          RoomType: "CREATE",
          RoomName: roomName,
          RoomPassword: roomPassword,
          Username: "eskil123",
        })
      );
    }
  }

  return (
    <div>
      <form onSubmit={handleSubmit}>
        <input
          value={roomName}
          onChange={(e) => setRoomName(e.target.value)}
          placeholder="Username"
        />
        <input
          value={roomPassword}
          onChange={(e) => setRoomPassword(e.target.value)}
          placeholder="Password"
        />
        <button>Confirm</button>
      </form>
    </div>
  );
}