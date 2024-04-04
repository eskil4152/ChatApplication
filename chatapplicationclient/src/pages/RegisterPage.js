import { useState } from "react";
import RegisterApi from "../requests/RegisterApi";

export default function RegisterPage() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  const [message, setMessage] = useState("");

  async function handleSubmit(e) {
    e.preventDefault();

    try {
      const res = await RegisterApi(username, password);

      if (res.ok) {
        setMessage("RES OK");
      } else {
        setMessage("RES NO OK");
      }
    } catch (e) {
      setMessage("error caugh: " + e);
    }
  }

  return (
    <div>
      <form onSubmit={handleSubmit}>
        <input
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          placeholder="Username"
        />
        <input
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          placeholder="Password"
        />
        <button>Confirm</button>
      </form>
      <p>{message}</p>
    </div>
  );
}
