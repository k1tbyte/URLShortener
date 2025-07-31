import {modal, useModalActions} from "@/components/Modal.tsx";
import {InputValidationWrapper} from "@/components/FieldWrapper.tsx";
import { Link } from "lucide-react";
import Input from "@/components/Input.tsx";
import {Button} from "@/components/Button.tsx";
import {useFormValidation, validators} from "@/hooks/useFormValidation.ts";
import {useForm} from "react-hook-form";
import {type FC, useState} from "react";
import axios from "axios";
import {LinksService} from "@/services/api/linksService.ts";

interface IAddNewUrlModalProps {
    onSuccess?: () => void;
}

export const AddNewUrlModal: FC<IAddNewUrlModalProps> = ({ onSuccess }) => {
    const [error, setError] = useState<string | null>(null);
    const {
        register,
        handleSubmit,
        resetField,
        formState: { isSubmitting}
    } = useForm<FormData>();

    const formValidation = useFormValidation([
        validators.url
    ])

    const { contentRef, closeModal } = useModalActions<HTMLDivElement>();

    const onAddNewUrl = async (data: FormData) => {
        setError(null);
        try {
            // @ts-ignore
            await LinksService.addNewUrl(data.url);
            closeModal();
            onSuccess?.();
        } catch (error) {
            // @ts-ignore
            resetField("url");
            setError(axios.isAxiosError(error) ?
                (error.response?.data || "An error occurred during URL creation") :
                "An unexpected error occurred during URL creation")
            return;
        }
    }

    return (
        <div ref={contentRef}>
            <form onSubmit={handleSubmit(onAddNewUrl)} ref={formValidation}>
                <InputValidationWrapper title={"Original url"} icon={<Link/>}>
                    {/* @ts-ignore */}
                    <Input {...register("url")} name={"url"}/>
                </InputValidationWrapper>

                { error && (
                    <div className="text-danger text-sm mt-2">
                        {error}
                    </div>
                )}

                <Button isLoading={isSubmitting} className="w-full mt-3 h-10" variant={"default"}>
                    Create
                </Button>
            </form>
        </div>

    );
}

export const openAddNewUrlModal = (onSuccess?: () => void) => {
    modal.open({
        body: <AddNewUrlModal onSuccess={onSuccess}/>,
        title: "Add New URL",
        className: "max-w-80 w-full"
    })
}