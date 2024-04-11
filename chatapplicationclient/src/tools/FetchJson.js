export async function fetchJSON(url) {
    const res = await fetch("http://localhost:5062" + url, {
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
