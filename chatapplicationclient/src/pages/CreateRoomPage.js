import CreateRoomApi from "../requests/CreateRoomApi";
import { useState } from "react";

export default function CreateRoomPage() {
  const [roomName, setRoomName] = useState("");
  const [roomPassword, setRoomPassword] = useState("");
  const [message, setMessage] = useState("");

  async function handleSubmit(e) {
    e.preventDefault();

    const data = await CreateRoomApi(roomName, roomPassword);

    console.log("data: " + data);
    //setMessage(data.status.toString());
  }

  return (
    <div>
      <form onSubmit={handleSubmit}>
        <input
          value={roomName}
          onChange={(e) => setRoomName(e.target.value)}
          placeholder="Room name"
        />
        <input
          value={roomPassword}
          onChange={(e) => setRoomPassword(e.target.value)}
          placeholder="Password"
        />
        <button>Confirm</button>
      </form>

      <p>{message}</p>
    </div>
  );
}
