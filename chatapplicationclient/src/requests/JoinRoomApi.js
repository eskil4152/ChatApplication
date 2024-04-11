export default async function JoinRoomApi(roomName, roomPassword) {
    return await fetch("http://localhost:5062/api/rooms/join", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            RoomName: roomName,
            RoomPassword: roomPassword,
        }),

        credentials: "include",
    });
}
