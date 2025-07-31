import {type FormEvent, useEffect, useRef} from "react";

const emailRegex = new RegExp('^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,4}$');
const loginRegex = new RegExp("^(?=.*[A-Za-z0-9])(?!.*[/*\\-.+_@!&$#%]{2})[A-Za-z0-9/*\\-.+_@!&$#%]{3,64}$")

export type TypeInputValidator = (input: string) => string | null;

const validate = (validator: TypeInputValidator | string, input: HTMLInputElement, message: Element | undefined, form: HTMLFormElement) => {
    let validateResult;

    if(typeof validator === 'string') {
        validateResult =
        (form.elements.namedItem(validator) as HTMLInputElement).value === input.value ? null : `The ${validator.toLowerCase()} does not match`;
    } else {
        validateResult = validator(input.value)
    }

    if(validateResult) {
        if(message) {
            message.textContent = validateResult
        }
        return false;
    }

    if(message && message.textContent != null) {
        message.textContent = null
    }
    return true;
}

export const useFormValidation = (validators: Array<TypeInputValidator | string>,
                                onSuccess?: (e:  FormEvent<HTMLFormElement>) => void) => {
    const formRef = useRef<HTMLFormElement>(null);

    useEffect(() => {
        if(!formRef.current) {
            return;
        }
        const form = formRef.current;

        const inputs = form.querySelectorAll('input')
        const messages = form.querySelectorAll('[validation-text]')
        const actions: (() => boolean)[] = [];
        for(let i = 0; i < inputs.length ; i++) {
            if(!validators[i]) {
                continue;
            }
            const callback = () => {
                return validate(validators[i], inputs[i], messages[i], form);
            };
            actions.push(callback)
            inputs[i].addEventListener('input', callback)
        }

        // @ts-ignore
        const onSubmit = (e) => {
            e.preventDefault();
            let success: boolean = true;
            for(const action of actions) {
                if(!action() && success) {
                    success = false;
                }
            }
            if(!success) {
                e.stopPropagation();
                return;
            }
            onSuccess?.(e)
        }

        form.addEventListener('submit', onSubmit)

        return () => {
            form.removeEventListener('submit', onSubmit)
            actions.forEach((o,i) => inputs[i].removeEventListener('input', o))
        }
    },[validators, onSuccess])

    return formRef;
}


export const validators: { [key: string]: TypeInputValidator  } = {
    required:  input =>  input.length == 0 ? "Required" : null,
    url: input => {
        try {
            new URL(input);
            return null;
        } catch (e) {
            return "Invalid URL";
        }
    },
    login: input => loginRegex.test(input) ? null : "Invalid login",
    password: input => input.length < 6 ? "Must be minimum 6 characters" : null,
    phone: input => input.length > 0 && input.length < 8 ? "Invalid phone number" : null,
    email: input => emailRegex.test(input) ? null : "Must be an email",
}
