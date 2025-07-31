import {useAuth} from "@/providers/authProvider.tsx";
import useSWR from "swr";
import {Loader} from "@/components/Loader.tsx";
import Input from "@/components/Input.tsx";
import {Button} from "@/components/Button.tsx";
import {openAddNewUrlModal} from "@/pages/modals/AddNewUrlModal.tsx";
import {type IShortUrl, LinksService} from "@/services/api/linksService.ts";
import type {FC} from "react";

interface IShortUrlCardProps {
    link: IShortUrl;
    mutate: () => void;
}

const ShortUrlCard: FC<IShortUrlCardProps> = ({ link, mutate } ) => {
    const auth = useAuth();

    const onDelete = () => {
        if (!auth.isAuthenticated || !link.owned) return;

        LinksService.deleteUrl(link.id)
            .then(() => {
                mutate()
            })
            .catch((error) => {
                console.error("Error deleting link:", error);
            });
    }

    return (
        <div className="mb-4 p-4 border border-border rounded-md bg-primary">
            <div className="text-sm text-gray-500">Original URL:</div>
            <a href={link.originalUrl} target="_blank" rel="noopener noreferrer"
               className="text-foreground-accent hover:underline">{link.originalUrl}</a>
            <div className="text-sm text-gray-500 mt-2">Short URL:</div>
            <a href={`/short/${link.shortUrl}`} target="_blank" rel="noopener noreferrer"
               className="text-secondary hover:underline">{link.shortUrl}</a>
            { auth.isAuthenticated && (
                <div className="w-full flex justify-end mt-2">
                    <div className="flex gap-2 ">
                        { (link.owned || auth.user?.role === "Admin") && (
                            <Button variant={"outlined"} className="text-danger border-danger hover:bg-danger/30" size={"sm"}
                                    onClick={onDelete}>
                                Delete
                            </Button>
                        )}
                        <Button variant={"outlined"} className="flex-center" size={"sm"} to={`/link/${link.id}`}>
                            Details
                        </Button>
                    </div>
                </div>
             )
            }

        </div>
    )
}


export const LinksPage = () => {
    const auth = useAuth(false);

    const { data, error, isLoading: swrLoading, mutate } = useSWR(
        "/links/getlinks",
        LinksService.getLinks
    );

    if (auth.isLoading || swrLoading) return <Loader className="mt-5"/>;
    if (error) return <div className="mt-5">Error: {error.message}</div>;

    return (
        <div className="w-full mt-4">
            <div className="w-full flex flex-col">
                <h1 className="text-2xl font-bold mb-2">All shortened links</h1>

                <div className="flex gap-2">
                    <Input placeholder="Search by original URL" className="rounded-md min-h-12"/>
                    { auth.isAuthenticated && (
                        <Button className="min-w-[10%]" onClick={() => {
                            openAddNewUrlModal(() => {
                                mutate(); // Refresh the list after adding a new URL
                            });
                        }}>
                            Add new
                        </Button>
                    )
                    }
                </div>
            </div>
            <div className="my-4 h-0.5 bg-primary w-full"></div>
            <div>
                { data?.length ? data.map((link) => (
                    <ShortUrlCard link={link} key={link.id} mutate={mutate}/>
                    )) :
                    <div className="mt-5">No links found</div>
                }
            </div>

        </div>
    );
};