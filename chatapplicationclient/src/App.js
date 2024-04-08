import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import { useState, useEffect } from "react";

import LoginPage from "./pages/LoginPage";
import JoinRoomPage from "./pages/JoinRoomPage";
import CreateRoomPage from "./pages/CreateRoomPage";
import AllRooms from "./pages/AllRoomsPage";
import ChatRoom from "./pages/ChatRoom";
import RegisterPage from "./pages/RegisterPage";

function App() {
    const [authenticated, setAuthenticated] = useState(false);
    const [loading, setLoading] = useState(true);

    async function Authenticate() {
        const result = await fetch("https://localhost:7025/api/authenticate", {
            method: "GET",
            credentials: "include",
        });

        return result.status;
    }

    useEffect(() => {
        async function fetchData() {
            try {
                const status = await Authenticate();
                setAuthenticated(status === 200);
            } catch (error) {
                console.error("Error fetching authentication status:", error);
                setAuthenticated(false);
            } finally {
                setLoading(false);
            }
        }

        fetchData();
    }, []);

    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        <BrowserRouter>
            <Routes>
                {authenticated ? (
                    <>
                        <Route path="/" element={<IndexLogged />} />

                        <Route path="/rooms" element={<AllRooms />} />
                        <Route path="/joinroom" element={<JoinRoomPage />} />
                        <Route
                            path="/createroom"
                            element={<CreateRoomPage />}
                        />

                        <Route path="/chat/:room" element={<ChatRoom />} />

                        <Route path="*" element={<NotFound />} />
                    </>
                ) : (
                    <>
                        <Route path="/login" element={<LoginPage />} />
                        <Route path="/register" element={<RegisterPage />} />
                        <Route path="*" element={<IndexNotLogged />} />
                    </>
                )}

                <Route path="*" element={<NotFound />} />
            </Routes>
        </BrowserRouter>
    );
}

function IndexNotLogged() {
    return (
        <div>
            <h1>Not logged in</h1>
            <Link to="/login">Log in</Link>
        </div>
    );
}

function IndexLogged() {
    return (
        <div>
            <h1>Logged in</h1>
        </div>
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
