import {forwardRef, useState} from "react";
import clsx from 'clsx';
import {Eye, EyeOff} from "lucide-react";
import Input, {type IInputProps} from "@/components/Input.tsx";

interface IPasswordBoxProps extends IInputProps { }

export const PasswordBox = forwardRef<HTMLInputElement, IPasswordBoxProps> (
    ({className, ...props}, ref) => {
    const [passwordVisible,setPasswordVisibility]= useState(false);

    return(
        <div className={clsx("w-full flex items-center bg-accent rounded-xm text-foreground text-2xs",className)}>
            <Input type={clsx(passwordVisible || "password","")} {...props} ref={ref}
                   className="bg-transparent"/>
            <button className="h-full hover:text-foreground-accent transition-all pr-2.5" type="button"
                    onClick={() => setPasswordVisibility((prev) => !prev)}>
                {
                    passwordVisible ? <Eye size={18} /> : <EyeOff size={18} />
                }
            </button>
        </div>
    )
})