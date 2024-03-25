export default async function JoinRoomApi(roomName, roomPassword) {
  return await fetch("/api/room/create", {
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
  /*
      if (response.status === 401 || 409 || 400) {
        throw new Error("Error");
      } else if (!response.ok) {
        throw new Error("Unknown error");
      }
    
      return response;
      */
}
