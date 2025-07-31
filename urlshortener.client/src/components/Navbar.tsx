import { Link, useLocation } from "react-router-dom";
import type {FC} from "react";
import clsx from "clsx";
import {Button} from "@/components/Button.tsx";
import {useAuth} from "@/providers/authProvider.tsx";

const NAV_ITEMS = [
    { to: '/links', label: 'Links' },
    { to: '/another', label: 'Another' },
];

interface INavItemProps {
    to: string;
    children: React.ReactNode;
}

const NavItem: FC<INavItemProps> = ({ children, to }) => {
    const location= useLocation();

    return (
        <Link to={to} className={clsx("text-sm transition-colors text-foreground px-4 py-1 rounded-md",
            { "text-foreground-accent bg-secondary": location.pathname === to })}>
            {children}
        </Link>
    );
}

export const Navbar = () => {
    const { isAuthenticated, logout } = useAuth();

    return (
        <nav className="flex w-full border-b border-border backdrop-primary justify-center py-2.5">
            <div className="container flex justify-between">
                <div className="flex gap-3">

                    { NAV_ITEMS.map((item) => (
                        <NavItem key={item.to} to={item.to}>
                            {item.label}
                        </NavItem>
                    ))
                    }
                    <Button variant={"outlined"} className="flex-center" onClick={() => {
                        window.open("/about", "_self");
                    }}>
                        About
                    </Button>
                </div>
                { isAuthenticated === false &&
                    <Button to={"/auth"} variant={"outlined"} className="text-md">
                        Login
                    </Button>
                }
                {
                    isAuthenticated &&
                    <Button onClick={logout} variant={"outlined"} className="text-md">
                        Logout
                    </Button>
                }

            </div>
        </nav>
    )
}