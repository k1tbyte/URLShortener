import {AuthService} from "@/services/api/authService.ts";

export interface IShortUrl {
    id: string;
    owned: boolean;
    originalUrl: string;
    shortUrl: string;
}

export interface IShortUrlFull {
    id: string;
    owned: boolean;
    originalUrl: string;
    shortCode: string;
    clicks: any[];
    createdAt: string;
    createdByUsername: string;
}

export class LinksService {
    public static async getLinks(): Promise<IShortUrl[]> {
        return AuthService.authorizedRequest<IShortUrl[]>({
            url: "/links/getlinks",
            method: "GET"
        });
    }

    public static addNewUrl(url: string) {
        console.log("adding new url", url);
        return AuthService.authorizedRequest({
            url: "/links/createLink",
            method: "POST",
            data: { url }
        });
    }

    public static deleteUrl(id: string) {
        return AuthService.authorizedRequest({
            url: `/links/deleteLink/${id}`,
            method: "DELETE",
            data: { id: Number(id) }
        });
    }

    public static getLinkById(id: string): Promise<IShortUrlFull> {
        return AuthService.authorizedRequest<IShortUrlFull>({
            url: `/links/getLinkById?id=${id}`,
            method: "GET"
        });
    }
}