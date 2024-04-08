export async function fetchJSON(url) {
    const res = await fetch("https://localhost:7025" + url, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
    });

    const response = await res.json();
    const status = res.status;

    return { response, status };
}
