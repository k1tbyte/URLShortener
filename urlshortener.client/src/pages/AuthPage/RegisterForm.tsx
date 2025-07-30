import {useForm} from "react-hook-form";
import {useFormValidation, validators} from "@/hooks/useFormValidation.ts";
import {InputValidationWrapper} from "@/components/FieldWrapper.tsx";
import {KeyRound, User} from "lucide-react";
import Input from "@/components/Input.tsx";
import {PasswordBox} from "@/components/PasswordBox.tsx";
import {Button} from "@/components/Button.tsx";
import {authApi} from "@/api/auth.tsx";
import {useState} from "react";
import {useNavigate} from "react-router-dom";
import {useAuth} from "@/providers/authProvider.tsx";

export const RegisterForm = () => {
    const {
        register,
        handleSubmit,
        formState: { isSubmitting}
    } = useForm<FormData>();
    const [error, setError] = useState<string | null>(null);
    let navigate = useNavigate();
    const auth = useAuth();

    const formValidation = useFormValidation([
        validators.login,
        validators.password,
        "password"

    ], (e) => {
        console.log(e)
    })

    const onRegister = async (data: FormData) => {
        setError(null);
        // @ts-ignore
        const response = await authApi.register(data);
        if (!response.ok) {
            setError(await response.text());
            return;
        }
        authApi.storeTokens(await response.json())
        auth.refresh()
        navigate("/links", {replace: true});
    }

    return (
        /* @ts-ignore */
        <form onSubmit={handleSubmit(onRegister)} ref={formValidation} className="w-full">
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

            <InputValidationWrapper className="mb-2 w-full"
                                    title="Confirm password"
                                    icon={<KeyRound size={18}/>}>
                {/* @ts-ignore */}
                <PasswordBox/>
            </InputValidationWrapper>
            { error && (
                <div className="text-danger text-sm mt-2">
                    {error}
                </div>
            )}

            <Button type={"submit"} className="w-full mt-7" variant={"default"} size={"sm"} isLoading={isSubmitting}>
                Register
            </Button>
        </form>
    )
}