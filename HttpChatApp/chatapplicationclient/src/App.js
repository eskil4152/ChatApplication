import { BrowserRouter, Routes, Route } from "react-router-dom";
import { WebSocketProvider } from "./WebSocketContext";

import LoginPage from "./pages/LoginPage";
import TestPage from "./pages/TestPage";
import Room from "./pages/Room";
import JoinRoomPage from "./pages/JoinRoomPage";

function App() {
  return (
    <WebSocketProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Room />} />
          <Route path="/roomjoin" element={<JoinRoomPage />} />
          <Route path="/test" element={<TestPage />} />
          <Route path="/login" element={<LoginPage />} />
        </Routes>
      </BrowserRouter>
    </WebSocketProvider>
  );
}

export default App;
