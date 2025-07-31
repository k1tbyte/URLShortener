import api from "@/services/api/apiClient.ts";
import type {IObservable} from "@/lib/observer/IObservable.ts";
import {EventEmitter} from "@/lib/observer/eventEmitter.ts";
import { jwtDecode } from 'jwt-decode';
import type {AxiosRequestConfig} from "axios";


export interface IUserInfo {
    username: string;
    userId: string;
    exp: number;
    role: string;
}

type TypeTokensResponse = {
    accessToken: string;
    refreshToken: string;
};

type TypeAuthRequest = {
    username: string;
    password: string;
};

export class AuthService {
    private static UserInfo: IUserInfo | undefined;
    public static readonly OnUserInfoChanged: IObservable<IUserInfo | undefined> = new EventEmitter();

    public static get userInfo(): IUserInfo | undefined {
        if (this.UserInfo) return this.UserInfo;

        const token = localStorage.getItem("accessToken");
        if (!token) return undefined;

        try {
            this.UserInfo = jwtDecode<IUserInfo>(token);
        } catch {
            return undefined;
        }

        return this.UserInfo;
    }

    private static storeTokens(tokens: TypeTokensResponse) {
        localStorage.setItem("accessToken", tokens.accessToken);
        localStorage.setItem("refreshToken", tokens.refreshToken);
        this.UserInfo = jwtDecode<IUserInfo>(tokens.accessToken);
        (this.OnUserInfoChanged as EventEmitter<IUserInfo | undefined>).emit(this.UserInfo);
    }

    public static async register(data: TypeAuthRequest): Promise<void> {
        const response = await api.post<TypeTokensResponse>("/auth/register", data);
        this.storeTokens(response.data);
    }

    public static async login(data: TypeAuthRequest): Promise<void> {
        const response = await api.post<TypeTokensResponse>("/auth/login", data);
        this.storeTokens(response.data);
    }

    public static async refresh(): Promise<void> {
        const accessToken = localStorage.getItem("accessToken");
        const refreshToken = localStorage.getItem("refreshToken");

        if (!accessToken || !refreshToken) {
            return;
        }

        const now = Math.floor(Date.now() / 1000);

        let decoded: IUserInfo;
        try {
            decoded = jwtDecode<IUserInfo>(accessToken);
        } catch (e) {
            this.logout();
            return;
        }

        if (decoded.exp > now + 10) {
            this.UserInfo = decoded;
            (this.OnUserInfoChanged as EventEmitter<IUserInfo | undefined>).emit(this.UserInfo);
            return;
        }

        try {
            const response = await api.post<TypeTokensResponse>("/auth/refreshSession", {
                accessToken,
                refreshToken,
            });

            this.storeTokens(response.data);
        } catch (err) {
            this.logout();
            throw err;
        }
    }

    public static logout() {
        api.post("/auth/logout", {
            accessToken: localStorage.getItem("accessToken"),
            refreshToken: localStorage.getItem("refreshToken")
        }).then(() => {
            console.log("Logged out successfully");
        }).catch((error) => {
            console.error("Logout error:", error);
        });
        localStorage.removeItem("accessToken");
        localStorage.removeItem("refreshToken");
        this.UserInfo = undefined;
        (this.OnUserInfoChanged as EventEmitter<IUserInfo | undefined>).emit(undefined);
    }

    public static async authorizedRequest<T = any>(config: AxiosRequestConfig<T>): Promise<T> {
        const token = localStorage.getItem("accessToken");

        if (token) {
            const decoded = this.userInfo;
            const now = Date.now() / 1000;

            // if the token is about to expire or has already expired, we update it
            if (decoded && decoded.exp - now < 30) {
                await this.refresh();
            }
        }

        try {
            const response = await api.request<T>({
                ...config,
                headers: {
                    ...(config.headers || {}),
                    Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
                }
            });
            return response.data;
        } catch (error: any) {
            if (error.response?.status === 401) {
                this.logout();
            }
            throw error;
        }
    }
}