import useLoading from "../tools/UseLoading";
import fetchJson from "../tools/FetchJson";

export default function GetAllRoomsApi() {
  const { loading, error, response } = useLoading(
    async () =>
      await fetch("/api/rooms/all", {
        method: "GET",
        credentials: "include",
      })
  );

  console.log("reS: " + response);

  return { loading, error, response };
}
