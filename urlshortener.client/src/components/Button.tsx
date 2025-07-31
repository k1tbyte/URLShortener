import type {ComponentPropsWithRef, FC } from "react";
import {Loader} from "@/components/Loader.tsx";
import {cva, type VariantProps} from "class-variance-authority";
import {cn} from "@/lib/utils.ts";
import {Link} from "react-router-dom";

const buttonVariants = cva(
    "disabled:pointer-events-none disabled:opacity-50 text-sm transition-colors",
    {
        variants: {
            variant: {
                default: "text-foreground-accent bg-secondary rounded-md hover:bg-secondary/80",
                outlined: "border-tertiary border text-secondary font-thin hover:bg-tertiary rounded",
                destructive: "bg-danger text-background font-bold rounded-md hover:bg-danger/80",
            },
            size: {
                default: "px-2",
                sm: "h-9 px-3",
                lg: "h-11 px-8",
            },
        },
        defaultVariants: {
            variant: "default",
            size: "default",
        },
    }
)

interface IButtonProps extends ComponentPropsWithRef<'button'>, VariantProps<typeof buttonVariants> {
    isLoading?: boolean;
    to?: string;
}

export const Button: FC<IButtonProps> = ({ children, isLoading,to, ref, variant, size, className, ...props }) => {
    const Comp = to ? Link : 'button';

    return (
        // @ts-ignore
        <Comp ref={ref} to={to}
                className={cn({ "pointer-events-none ": isLoading }, buttonVariants({ variant, size, className }))}
                {...props}>
            {isLoading ? <div className="w-full h-full flex-center">
                <Loader/>
            </div> : children}
        </Comp>
    )
}