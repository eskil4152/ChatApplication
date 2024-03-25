import { BrowserRouter, Routes, Route } from "react-router-dom";

import LoginPage from "./pages/LoginPage";
import JoinRoomPage from "./pages/JoinRoomPage";
import CreateRoomPage from "./pages/CreateRoomPage";
import AllRooms from "./pages/AllRoomsPage";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<LoginPage />} />

        <Route path="/joinroom" element={<JoinRoomPage />} />
        <Route path="/createroom" element={<CreateRoomPage />} />
        <Route path="/rooms" element={<AllRooms />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
