import {createContext, type FC, type ReactNode, useContext, useEffect, useState} from "react";
import {AuthService, type IUserInfo} from "@/services/api/authService.ts";
import {useNavigate} from "react-router-dom";

interface IAuthContextType {
    isAuthenticated?: boolean | undefined;
    user?: IUserInfo;
    logout: () => void;
}

const AuthContext = createContext<IAuthContextType>(undefined!);


export const AuthProvider: FC<{ children: ReactNode }> = ({ children }) => {
    const [user, setUser] = useState<IUserInfo | undefined>(undefined);
    const [loading, setLoading] = useState<boolean>(true);
    const navigate = useNavigate();


    // Initialize authentication state
    useEffect(() => {

        const onUserChanged = (userInfo: IUserInfo | undefined) => {
            console.log("User info changed:", userInfo);
            setUser(userInfo);
        }

        AuthService.OnUserInfoChanged.subscribe(onUserChanged);

        AuthService.refresh().finally(() => {
            setLoading(false);
        })

        return () => {
            AuthService.OnUserInfoChanged.unsubscribe(onUserChanged);
        }
    }, []);

    const logout = () => {
        AuthService.logout();
        navigate("/auth", {replace: true});
    }

    return (
        <AuthContext.Provider value={{
            user,
            isAuthenticated: loading ? undefined : !!user,
            logout
        }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = (required: boolean = false) => {
    const navigate = useNavigate();
    const context = useContext(AuthContext);

    const isLoading = context.isAuthenticated === undefined;

    useEffect(() => {
        if (context.isAuthenticated === false && required) {
            navigate("/auth", { replace: true });
        }
    }, [context.isAuthenticated, required, navigate]);

    return {
        ...context,
        isLoading,
    };
};