export default async function LoginApi(username, password) {
    return await fetch("http://localhost:5062/api/login", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            Username: username,
            Password: password,
        }),

        credentials: "include",
    });
}
