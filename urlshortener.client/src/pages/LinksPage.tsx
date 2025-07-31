import {useAuth} from "@/providers/authProvider.tsx";
import {AuthService} from "@/services/api/authService.ts";
import useSWR from "swr";

export const LinksPage = () => {
    const auth = useAuth(true);

    const { data, error, isLoading: swrLoading } = useSWR(
        auth.isLoading ? null : "/links/getlinks",
        (url) => AuthService.authorizedRequest({ url, method: "GET" })
    );

    if (auth.isLoading || swrLoading) return <div>Loading...</div>;
    if (error) return <div>Error: {error.message}</div>;

    return (
        <div className="h-full">
            Hello, this is the Links Page!
            <pre>{JSON.stringify(data, null, 2)}</pre>
        </div>
    );
};