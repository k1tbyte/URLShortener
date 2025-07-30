import {useEffect} from "react";
import {useAuth} from "@/providers/authProvider.tsx";

export const LinksPage = () => {
    const auth = useAuth();

    useEffect(() => {
        if(auth.isAuthenticated) {
            auth.authorizedRequest("/api/links/getlinks", {
                method: "GET",
                headers: {
                    "Content-Type": "application/json"
                }
            }).then(async response => {
                if (!response.ok) {
                    throw new Error("Failed to fetch links");
                }
                console.log(await response.json());
            })
        }
    }, [auth.isAuthenticated]);


     return (
        <div className="h-full">
            Hello, this is the Links Page!
        </div>
     )
}