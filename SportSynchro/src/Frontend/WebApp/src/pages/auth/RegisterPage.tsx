import { Formik, Form, Field, ErrorMessage } from "formik";
import * as Yup from "yup";
import { useNavigate } from "react-router";

type ApiErrorResponse = {
    error?: string;
};

const RegisterSchema = Yup.object({
    username: Yup.string().required("Username is required."),
    email: Yup.string()
        .email("Invalid email address.")
        .required("Email is required."),
    password: Yup.string()
        .min(6, "Password must be at least 6 characters long.")
        .matches(/[A-Z]/, "Password must contain at least one uppercase letter.")
        .matches(/[a-z]/, "Password must contain at least one lowercase letter.")
        .matches(/[0-9]/, "Password must contain at least one number.")
        .matches(/[^A-Za-z0-9]/, "Password must contain at least one special character.")
        .required("Password is required."),
    confirmPassword: Yup.string()
        .oneOf([Yup.ref("password")], "Passwords do not match.")
        .required("Confirm password is required."),
});

export function RegisterPage() {
    const authBaseUrl = import.meta.env.VITE_AUTH_AUTHORITY;
    const navigate = useNavigate();

    return (
        <div className="min-h-screen flex items-center justify-center bg-linear-to-br from-slate-900 to-slate-950 px-4">
            <div className="w-full max-w-md bg-white rounded-2xl shadow-2xl p-8">

                {/* Header */}
                <div className="text-center mb-6">
                    <h1 className="text-2xl font-bold tracking-tight text-slate-900">
                        SportSynchro
                    </h1>
                    <p className="text-sm text-slate-500 mt-1">
                        Create your account
                    </p>
                </div>

                <Formik
                    initialValues={{
                        username: "",
                        email: "",
                        password: "",
                        confirmPassword: "",
                    }}
                    validationSchema={RegisterSchema}
                    onSubmit={async (values, { setSubmitting, setStatus }) => {
                        setStatus(null);

                        try {
                            const response = await fetch(`${authBaseUrl}/auth/register`, {
                                method: "POST",
                                headers: {
                                    "Content-Type": "application/json",
                                },
                                body: JSON.stringify({
                                    username: values.username,
                                    email: values.email,
                                    password: values.password,
                                }),
                            });

                            if (!response.ok) {
                                let message = "Registration failed.";

                                try {
                                    const body: ApiErrorResponse = await response.json();
                                    if (body.error) {
                                        message = body.error;
                                    }
                                } catch {
                                    // ignore parse errors
                                }

                                throw new Error(message);
                            }

                            navigate("/", { replace: true });
                        } catch (err: unknown) {
                            if (err instanceof Error) {
                                setStatus(err.message);
                            } else {
                                setStatus("An unexpected error occurred.");
                            }
                        } finally {
                            setSubmitting(false);
                        }
                    }}
                >
                    {({ isSubmitting, status }) => (
                        <Form className="space-y-4">

                            {/* Username */}
                            <div>
                                <label className="block text-sm font-medium text-slate-700">
                                    Username
                                </label>
                                <Field
                                    name="username"
                                    className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                                />
                                <ErrorMessage
                                    name="username"
                                    component="p"
                                    className="text-sm text-red-600 mt-1"
                                />
                            </div>

                            {/* Email */}
                            <div>
                                <label className="block text-sm font-medium text-slate-700">
                                    Email
                                </label>
                                <Field
                                    name="email"
                                    type="email"
                                    className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                                />
                                <ErrorMessage
                                    name="email"
                                    component="p"
                                    className="text-sm text-red-600 mt-1"
                                />
                            </div>

                            {/* Password */}
                            <div>
                                <label className="block text-sm font-medium text-slate-700">
                                    Password
                                </label>
                                <Field
                                    name="password"
                                    type="password"
                                    className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                                />
                                <ErrorMessage
                                    name="password"
                                    component="p"
                                    className="text-sm text-red-600 mt-1"
                                />
                            </div>

                            {/* Confirm password */}
                            <div>
                                <label className="block text-sm font-medium text-slate-700">
                                    Confirm password
                                </label>
                                <Field
                                    name="confirmPassword"
                                    type="password"
                                    className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                                />
                                <ErrorMessage
                                    name="confirmPassword"
                                    component="p"
                                    className="text-sm text-red-600 mt-1"
                                />
                            </div>

                            {/* Server error */}
                            {status && (
                                <p className="text-sm text-red-600 text-center">
                                    {status}
                                </p>
                            )}

                            <button
                                type="submit"
                                disabled={isSubmitting}
                                className="w-full rounded-lg bg-indigo-600 py-2.5 text-sm font-semibold text-white hover:bg-indigo-700 transition disabled:opacity-60"
                            >
                                Create account
                            </button>
                        </Form>
                    )}
                </Formik>

                {/* Footer */}
                <div className="mt-6 text-center text-sm text-slate-500">
                    Already have an account?{" "}
                    <a href="/" className="font-medium text-indigo-600 hover:underline">
                        Sign in
                    </a>
                </div>
            </div>
        </div>
    );
}
