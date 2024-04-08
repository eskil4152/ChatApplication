import { fetchJSON } from "../tools/FetchJson";

export default async function GetAllRoomsApi() {
    return await fetchJSON("/api/rooms/all");
}
