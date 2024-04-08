import JoinRoomApi from "../requests/JoinRoomApi";
import { useState } from "react";

export default function JoinRoomPage() {
    const [roomName, setRoomName] = useState("");
    const [roomPassword, setRoomPassword] = useState("");

    const [error, setError] = useState("");

    async function handleSubmit(e) {
        e.preventDefault();

        if (!roomName) {
            setError("Please enter a name");
            return;
        } else if (!roomPassword) {
            setError("Please enter a password");
            return;
        }

        const data = await JoinRoomApi(roomName, roomPassword);

        if (data.status === 200) {
            window.location.href = "rooms";
        } else if (data.status === 404) {
            setError("Room with name '" + roomName + "' not found");
        } else if (data.status === 401 || data.status === 403) {
            setError("Unauthorized or forbidden");
        } else {
            setError("Unknown error occured");
        }
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

            <p>{error}</p>
        </div>
    );
}
