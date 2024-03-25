import { postJSON } from "../tools/FetchJson";

export default async function CreateRoomApi(roomName, roomPassword) {
  return await fetch("http://localhost:8083/api/room/create", {
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
