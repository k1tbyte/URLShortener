import {createBrowserRouter, Navigate, RouterProvider} from 'react-router-dom'
import {MainLayout} from "./layouts/MainLayout.tsx";
import { AuthPage } from "./pages/AuthPage/AuthPage.tsx";
import {LinksPage} from "./pages/LinksPage.tsx";
import {ModalsHost} from "@/components/Modal.tsx";

const router = createBrowserRouter([
    {
        path: "/",
        element: <MainLayout/>,
        children: [
            {
                index: true,
                element: <Navigate to="/auth" replace/>
            },
            {
                path: "/auth",
                element: <AuthPage/>
            },
            {
                path: "/links",
                element: <LinksPage/>
            }
        ]
    }
]);

function App() {
    return (
        <>
            <RouterProvider router={router}/>
            <ModalsHost/>
        </>

    );
}

export default App;