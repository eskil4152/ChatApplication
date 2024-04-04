import GetAllRooms from "../requests/GetAllRoomsApi";

export default function AllRooms() {
  const { loading, error, response } = GetAllRooms();

  while (loading) {
    return (
      <div>
        <h1>Loading...</h1>
      </div>
    );
  }

  if (response?.status === 200) {
    return (
      <div>
        <h1>OK</h1>
      </div>
    );
  } else if (error) {
    return (
      <div>
        <h1>Error: {error.message}</h1>
      </div>
    );
  } else if (response?.status === 401) {
    return (
      <div>
        <p style={{ color: "red" }}>Error: 401</p>
      </div>
    );
  } else {
    return (
      <div>
        <p style={{ color: "red" }}>Error: Unknown error occured</p>
      </div>
    );
  }
}
