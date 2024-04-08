import GetAllRoomsApi from "../requests/GetAllRoomsApi";
import { useLoading } from "../tools/UseLoading";

export default function AllRooms() {
    const { loading, error, response } = useLoading(
        async () => await GetAllRoomsApi()
    );

    if (loading) {
        return (
            <div>
                <h1>Loading...</h1>
            </div>
        );
    }

    if (response?.status === 401 || response?.status === 403) {
        return (
            <div>
                <p style={{ color: "red" }}>Error: 401/403</p>
            </div>
        );
    } else if (error) {
        return (
            <div>
                <h1>Error: {error}</h1>
            </div>
        );
    } else if (response?.status !== 200) {
        return (
            <div>
                <p style={{ color: "red" }}>Error: Unknown error occured</p>
            </div>
        );
    }

    return (
        <div>
            <h1>OK</h1>
            {response.response.map((room) => (
                <div>
                    <h3>{room.roomName}</h3>
                    <button
                        onClick={() => {
                            window.location.href = "/chat/" + room.roomName;
                        }}
                    >
                        Join
                    </button>
                </div>
            ))}
        </div>
    );
}
