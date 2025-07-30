import {type Context, createContext, type FC, type PropsWithChildren, useContext} from "react";
import {type NavigateFunction, useNavigate} from "react-router-dom";

// @ts-ignore
const RouterContext: Context<NavigateFunction> = createContext(null!);

export const RouterProvider: FC<PropsWithChildren> = ({ children }) => {
    const router = useNavigate();
    return (
        <RouterContext.Provider value={router}>
            {children}
        </RouterContext.Provider>
    );
};

export const useRouter= () => useContext(RouterContext);