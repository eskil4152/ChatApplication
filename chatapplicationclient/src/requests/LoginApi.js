export default async function LoginApi(username, password) {
  return await fetch("https://localhost:7025/api/login", {
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
