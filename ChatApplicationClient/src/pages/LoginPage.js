import { useState } from "react";
import { Link } from "react-router-dom";

export default function LoginPage() {
  const [loggedIn, setLoggedIn] = useState(false);

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  function handleSubmit(e) {
    e.preventDefault();

    console.log("Username: " + username);
    console.log("Pword: " + password);

    setLoggedIn(!loggedIn);
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

      <Link to={"/test"}>BUTTON</Link>

      {loggedIn ? (
        <div>
          <p>Logged in</p>
        </div>
      ) : (
        ""
      )}
    </div>
  );
}
