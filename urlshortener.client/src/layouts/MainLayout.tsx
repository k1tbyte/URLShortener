import type { FC } from "react";
import {Navbar} from "../components/Navbar.tsx";
import {Outlet } from "react-router-dom";
import {RouterProvider} from "../providers/RouterProvider.tsx";
import {AuthProvider} from "@/providers/authProvider.tsx";

export const MainLayout: FC = ( ) => {
    return (
        <RouterProvider>
            <AuthProvider>
                <div className="flex w-full h-svh flex-col justify-center items-center">
                    <Navbar/>
                    <div className="h-full w-full container">
                        <Outlet/>
                    </div>

                </div>
            </AuthProvider>
        </RouterProvider>
    )
}