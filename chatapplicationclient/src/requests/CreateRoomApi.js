export default async function CreateRoomApi(roomName, roomPassword) {
    return await fetch("https://localhost:7025/api/room/create", {
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
