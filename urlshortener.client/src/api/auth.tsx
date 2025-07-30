
export const authApi = {
    register(data: { username: string; password: string }) {
        return fetch("/api/auth/register", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(data),
        });
    },
    login(data: { username: string; password: string }) {
        return fetch("/api/auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(data),
        });
    },
    async refresh() {
        if( localStorage.getItem("refreshing") === "true") {
            return;
        }

        try {
            localStorage.setItem("refreshing", "true");
            const accessToken = localStorage.getItem("accessToken");
            const refreshToken = localStorage.getItem("refreshToken");

            if (!accessToken || !refreshToken) {
                return Promise.reject(new Error("No tokens found"));
            }

            const response = await fetch("/api/auth/refreshSession", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ accessToken, refreshToken }),
            });
            if (!response.ok) {
                localStorage.removeItem("accessToken");
                localStorage.removeItem("refreshToken");
                return Promise.reject(new Error("Failed to refresh tokens"));
            }
            const tokens = await response.json();
            this.storeTokens(tokens);
        } finally {
            localStorage.removeItem("refreshing");
        }
    },
    storeTokens(tokens: { accessToken: string; refreshToken: string }) {
        console.log("Storing tokens", tokens);
        localStorage.setItem("accessToken", tokens.accessToken);
        localStorage.setItem("refreshToken", tokens.refreshToken);
    },
    async logout() {
        try {
            await fetch("/api/auth/logout", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${localStorage.getItem("accessToken")}`
                },
                body: JSON.stringify({
                    refreshToken: localStorage.getItem("refreshToken"),
                })
            });
        } catch (error) {
            console.error("Error during sign out:", error);
        } finally {
            localStorage.removeItem("accessToken");
            localStorage.removeItem("refreshToken");
        }
    }
}