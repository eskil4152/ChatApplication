export class HttpError extends Error {
  constructor(status, statusText) {
    super("My custom exception" + statusText);
    this.status = status;
  }
}

export default async function fetchJSON(url) {
  const response = await fetch("https://localhost:7025" + url, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
    credentials: "include",
  });

  const data = await response.json();
  const status = response.status;

  return { status, data };
}

export async function postJSON(url, content) {
  try {
    const response = await fetch("http://localhost:7025" + url, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(content),
    });

    const status = response.status;

    const contentType = response.headers.get("content-type");

    const isJsonResponse =
      contentType && contentType.includes("application/json");

    if (isJsonResponse) {
      const data = await response.json();
      return { status, data };
    } else {
      return { status };
    }
  } catch (error) {
    console.error("Error:", error);
    throw error;
  }
}
