import { useState } from "react";
import JoinRoom from "../requests/JoinRoomApi";

export default function JoinRoomPage() {
    const [roomName, setRoomName] = useState("");
    const [roomPassword, setRoomPassword] = useState("");
    const [message, setMessage] = useState("");

    async function handleSubmit(e) {
        e.preventDefault();

        const data = await JoinRoom(roomName, roomPassword);

        setMessage(data.status.toString());
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
