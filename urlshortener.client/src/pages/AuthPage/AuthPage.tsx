import {Tab, TabPanel} from "@/components/Tabs.tsx";
import {LoginForm} from "@/pages/AuthPage/LoginForm.tsx";
import {RegisterForm} from "@/pages/AuthPage/RegisterForm.tsx";
import {useAuth} from "@/providers/authProvider.tsx";
import {Loader} from "@/components/Loader.tsx";
import {useNavigate} from "react-router-dom";
import {useEffect} from "react";

export const AuthPage = () => {
    let navigate = useNavigate();
    const { user, isAuthenticated } = useAuth();

    useEffect(() => {
        if(isAuthenticated) {
            navigate("/links", {replace: true});
        }
    }, [user]);

    if (isAuthenticated !== false) {
        return <div className="h-full flex items-center justify-center">
            <Loader/>
        </div>;
    }

    return (
        <div className="h-full flex items-center max-w-96 w-full">
            <div className="backdrop-primary px-6 py-10 w-full rounded-xl">
                <h1 className="text-2xl text-center text-foreground-accent font-bold ">Authentication</h1>
                <div className="h-0.5 bg-background w-full my-3"></div>
                <TabPanel activeKey={"login"} className="bg-primary border-tertiary rounded-xl mb-1">
                    <Tab title={"Login"} key={"login"}>
                        <LoginForm/>
                    </Tab>
                    <Tab title={"Register"} key={"register"}>
                        <RegisterForm/>
                    </Tab>
                </TabPanel>
            </div>
        </div>
    )
}