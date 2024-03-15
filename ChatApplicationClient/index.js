const ws = new WebSocket("localhost:8081");

ws.addEventListener("open", function (event) {
  console.log("Conntected to WebSocket server");
});

ws.addEventListener("message", function (event) {
  console.log("Message received:", event.data);
});

ws.addEventListener("close", function (event) {
  console.log("Disconnected from WebSocket server");
});

function sendMessage(message) {
  if (ws.readyState === WebSocket.OPEN) {
    ws.send(message);
    console.log("Message sent:", message);
  } else {
    console.error("WebSocket connection is not open");
  }
}

window.addEventListener("load", function () {
  sendMessage(
    JSON.stringify({
      username: "user123",
      password: "password123",
      roomNumber: 1,
      message: "Hello from client!",
    })
  );
});
