export default async function CreateRoomApi(roomName, roomPassword) {
    return await fetch("http://localhost:5062/api/rooms/create", {
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
