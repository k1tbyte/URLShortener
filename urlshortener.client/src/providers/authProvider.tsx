import {createContext, type FC, type ReactNode, useContext, useEffect, useState} from "react";
import { jwtDecode } from 'jwt-decode';
import {authApi} from "@/api/auth.tsx";
import {useNavigate} from "react-router-dom";

interface IUserInfo {
    username: string;
    userId: string;
    exp: number;
    role: string;
}

interface IAuthContextType {
    isAuthenticated?: boolean | undefined;
    user?: IUserInfo;
    refresh: () => Promise<boolean>;
    logout: () => Promise<void>;
    authorizedRequest: (request: Request | URL | string, options?: RequestInit) => Promise<Response>;
    loading: boolean;
}

const AuthContext = createContext<IAuthContextType>(undefined!);


export const AuthProvider: FC<{ children: ReactNode }> = ({ children }) => {
    const [user, setUser] = useState<IUserInfo | undefined>(undefined);
    const [loading, setLoading] = useState<boolean>(true);
    const navigate = useNavigate();

    // Helper function to decode token and validate
    const decodeToken = (token: string | null): IUserInfo | null => {
        if (!token) return null;
        try {
            const decoded = jwtDecode<IUserInfo>(token);
            return decoded && decoded.exp * 1000 > Date.now() ? decoded : null;
        } catch {
            return null;
        }
    };

    // Handles token refresh
    const refreshToken = async (): Promise<boolean> => {
        try {
            await authApi.refresh();
            const newToken = localStorage.getItem("accessToken");
            const newUser = decodeToken(newToken);

            if (newUser) {
                setUser(newUser);
                return true;
            }
        } catch (error) {
            console.error("Failed to refresh token:", error);
        }

        // If we got here, refresh failed
        localStorage.removeItem("accessToken");
        setUser(undefined);
        return false;
    };

    // Refreshes user state based on token
    const refresh = async (): Promise<boolean> => {
        const token = localStorage.getItem("accessToken");
        const decodedUser = decodeToken(token);

        if (decodedUser) {
            setUser(decodedUser);
            return true;
        } else if (token) {
            // Token exists but is invalid or expired - try to refresh
            return await refreshToken();
        }

        setUser(undefined);
        return false;
    };

    const logout = async () => {
        try {
            await authApi.logout();
            navigate("/auth", { replace: true });
        } catch (error) {
            console.error("Logout error:", error);
        } finally {
            localStorage.removeItem("accessToken");
            setUser(undefined);
        }
    }

    // Enhanced authorized request with automatic token refresh
    const authorizedRequest = async (request: Request | URL | string, options?: RequestInit): Promise<Response> => {
        // Check if current token is valid
        const token = localStorage.getItem("accessToken");
        const decodedUser = decodeToken(token);

        // If token is expired but exists, try refreshing
        if (!decodedUser && token) {
            const refreshSuccess = await refreshToken();
            if (!refreshSuccess) {
                throw new Error("Authentication failed");
            }
        }

        // Get the (potentially new) token
        const accessToken = localStorage.getItem("accessToken");
        if (!accessToken) {
            throw new Error("No access token found");
        }

        const headers = new Headers(options?.headers);
        headers.set("Authorization", `Bearer ${accessToken}`);

        return fetch(request, {
            ...options,
            headers
        });
    };

    // Initialize authentication state
    useEffect(() => {
        refresh()
            .finally(() => setLoading(false));
    }, []);

    return (
        <AuthContext.Provider value={{
            user,
            isAuthenticated: loading ? undefined : !!user,
            loading,
            refresh,
            logout,
            authorizedRequest
        }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);