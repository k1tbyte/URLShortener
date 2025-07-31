import useSWR from "swr";
import {useNavigate, useParams} from "react-router-dom";
import {LinksService} from "@/services/api/linksService.ts";
import {Loader} from "@/components/Loader.tsx";
import {useAuth} from "@/providers/authProvider.tsx";
import {useEffect} from "react";
import {Button} from "@/components/Button.tsx";

export const LinkInfoPage = () => {

    const { id } = useParams();
    const navigate = useNavigate();

    const auth = useAuth(true);

    const { error, isLoading, data } = useSWR( auth.isAuthenticated ? "/api/links/getLinkById/"+id : null,
        () => {
        return LinksService.getLinkById(id!)!
    })

    useEffect(() => {
        if (error || auth.isAuthenticated === false) {
            navigate("/links", {replace: true});
        }
    }, [auth.isAuthenticated, error]);

    if (isLoading || auth.isLoading || !data) {
        return <div className="w-full h-full flex-center">
            <Loader/>
        </div>
    }

    return (
        <div className="w-full h-full flex-center flex-col">
            <div className="w-full max-w-96">
                <div className="backdrop-primary px-6 py-10 w-full rounded-xl ">
                    <h1 className="text-2xl text-center text-foreground-accent font-bold mb-2">Link Information</h1>
                    <div className="bg-background h-0.5 mb-3"></div>
                    <p className="text-sm text-foreground">Original URL:</p>
                    <a
                        href={data.originalUrl}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="text-foreground-accent hover:underline overflow-hidden text-ellipsis whitespace-nowrap block w-full"
                    >
                        {data.originalUrl}
                    </a>
                    <p className="text-sm text-foreground mt-2">Short URL:</p>
                    <a href={`/link/${data.shortCode}`} target="_blank" rel="noopener noreferrer"
                       className="text-secondary hover:underline">
                        {window.location.origin}/short/{data.shortCode}
                    </a>
                    <p className="text-sm text-foreground mt-2">Created At:</p>
                    <div className="text-secondary">{new Date(data.createdAt).toLocaleString()}</div>
                    <p className="text-sm text-foreground mt-2">Created by:</p>
                    <div className="text-secondary">{data.createdByUsername || "Unknown"}</div>
                    <p className="text-sm text-foreground mt-2">
                        ID:
                        <span className="text-secondary"> {data.id}</span>
                    </p>
                </div>

                <Button className={"w-full mt-5 h-10"} onClick={() => navigate("/links", {replace: true})}>
                    Back
                </Button>
            </div>
        </div>
    )
}