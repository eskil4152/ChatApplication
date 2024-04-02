import { BrowserRouter, Routes, Route } from "react-router-dom";

import LoginPage from "./pages/LoginPage";
import JoinRoomPage from "./pages/JoinRoomPage";
import CreateRoomPage from "./pages/CreateRoomPage";
import AllRooms from "./pages/AllRoomsPage";
import ChatRoom from "./pages/ChatRoom";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<LoginPage />} />

        <Route path="/chat" element={<ChatRoom />} />
        <Route path="/joinroom" element={<JoinRoomPage />} />
        <Route path="/createroom" element={<CreateRoomPage />} />
        <Route path="/rooms" element={<AllRooms />} />

        <Route path="/*" element={<NotFound />} />
      </Routes>
    </BrowserRouter>
  );
}

function NotFound() {
  return (
    <div>
      <h1>Not found</h1>
    </div>
  );
}

export default App;
