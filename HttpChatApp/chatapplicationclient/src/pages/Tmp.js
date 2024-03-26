export default function Tmp() {
  const roomName = "Room";
  fetch(`https://localhost:7025/api/rooms/enter?roomName=${roomName}`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      // Include any necessary headers for authentication, e.g., cookies
    },
  })
    .then((response) => {
      if (response.ok) {
        console.log("Was ok");
        // WebSocket connection establishment
        const webSocket = new WebSocket(`wss://localhost:7025/ws/${roomName}`);

        webSocket.onopen = function (event) {
          console.log("WebSocket connection established.");
          // You can perform any additional actions here after the WebSocket connection is established
        };

        webSocket.onmessage = function (event) {
          // Handle incoming WebSocket messages
          console.log("Received message:", event.data);
        };

        webSocket.onclose = function (event) {
          console.log("WebSocket connection closed.");
        };

        webSocket.onerror = function (event) {
          console.error("WebSocket error:", event);
        };
      } else {
        console.error("Failed to enter the room. Status:", response.status);
      }
    })
    .catch((error) => {
      console.error("An error occurred while entering the room:", error);
    });

  return (
    <div>
      <h1>Hello</h1>
    </div>
  );
}
