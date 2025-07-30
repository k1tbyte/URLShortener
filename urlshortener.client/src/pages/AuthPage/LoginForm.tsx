import Input from "@/components/Input.tsx";
import {useForm} from "react-hook-form";
import { KeyRound, User} from "lucide-react";
import {InputValidationWrapper} from "@/components/FieldWrapper.tsx";
import {PasswordBox} from "@/components/PasswordBox.tsx";
import {useFormValidation, validators} from "@/hooks/useFormValidation.ts";
import {Button} from "@/components/Button.tsx";
import {authApi} from "@/api/auth.tsx";
import {useState} from "react";
import {useNavigate} from "react-router-dom";
import {useAuth} from "@/providers/authProvider.tsx";

export const LoginForm = () => {
    const {
        register,
        handleSubmit,
        resetField,
        formState: { isSubmitting}
    } = useForm<FormData>();
    const [error, setError] = useState<string | null>(null);
    let navigate = useNavigate();
    const auth = useAuth();

    const formValidation = useFormValidation([
        validators.login,
        validators.password,
    ], (e) => {
        console.log(e)
    })

    const onLogin = async (data: FormData) => {
        setError(null);
        // @ts-ignore
        const response = await authApi.login(data);
        if (!response.ok) {
            const errorText = await response.text();
            // @ts-ignore
            resetField("password");
            setError(errorText);
            return;
        }
        authApi.storeTokens(await response.json())
        auth.refresh()
        navigate("/links", {replace: true});
    }

    return (
        /* @ts-ignore */
        <form onSubmit={handleSubmit(onLogin)} ref={formValidation} className="w-full">
            <InputValidationWrapper className="mb-2 w-full"
                                    title="Login"
                                    icon={<User size={18}/>}>
                {/* @ts-ignore */}
                <Input {...register("login")} name="login"/>
            </InputValidationWrapper>
            <InputValidationWrapper className="mb-2 w-full"
                                    title="Password"
                                    icon={<KeyRound size={18}/>}>
                {/* @ts-ignore */}
                <PasswordBox {...register("password")} name="password"/>
            </InputValidationWrapper>
            { error && (
                <div className="text-danger text-sm mt-2">
                    {error}
                </div>
            )}
            <Button type={"submit"} className="w-full mt-7" variant={"default"} size={"sm"} isLoading={isSubmitting}>
                Continue
            </Button>
        </form>
    )
}