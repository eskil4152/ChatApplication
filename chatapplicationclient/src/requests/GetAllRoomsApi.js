import useLoading from "../tools/UseLoading";
import fetchJson from "../tools/FetchJson";

export default function GetAllRoomsApi() {
  const { loading, error, response } = useLoading(
    async () =>
      await fetch("https://localhost:7025/api/rooms/all", {
        method: "GET",
        credentials: "include",
      })
  );

  return { loading, error, response };
}
