import {LoaderCircle} from "lucide-react";
import type {FC} from "react";
import {cn} from "@/lib/utils.ts";

export const Loader: FC<{className?: string}> = ({ className }) => {
   return (
       <LoaderCircle className={cn("animate-spin", className)}/>
   )
}