import { useEffect, useState } from "react";

export function useLoading(loadingFunction, deps = []) {
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState();
    const [response, setResponse] = useState();

    async function load() {
        try {
            setLoading(true);
            setResponse(await loadingFunction());
        } catch (error) {
            setError(error);
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        load();
    }, deps);

    return { loading, error, response, reload: load };
}
