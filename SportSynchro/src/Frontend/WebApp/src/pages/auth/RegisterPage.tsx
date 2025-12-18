import { useState } from "react";
import { useNavigate } from "react-router";

export function RegisterPage() {
    const navigate = useNavigate();
    const [form, setForm] = useState({
        username: "",
        email: "",
        password: "",
        confirmPassword: ""
    });

    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    const onSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);

        if (form.password !== form.confirmPassword) {
            setError("Passwords do not match");
            return;
        }

        setLoading(true);

        const response = await fetch(
            "https://localhost:5001/api/auth/register",
            {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    username: form.username,
                    email: form.email,
                    password: form.password
                })
            }
        );

        if (!response.ok) {
            const body = await response.json();
            setError(body.error ?? "Registration failed");
            setLoading(false);
            return;
        }

        navigate("/", { replace: true });
    };

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

                {/* Form */}
                <form onSubmit={onSubmit} className="space-y-4">
                    <div>
                        <label className="block text-sm font-medium text-slate-700">
                            Username
                        </label>
                        <input
                            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                            value={form.username}
                            onChange={e =>
                                setForm({ ...form, username: e.target.value })
                            }
                            required
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-slate-700">
                            Email
                        </label>
                        <input
                            type="email"
                            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                            value={form.email}
                            onChange={e =>
                                setForm({ ...form, email: e.target.value })
                            }
                            required
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-slate-700">
                            Password
                        </label>
                        <input
                            type="password"
                            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                            value={form.password}
                            onChange={e =>
                                setForm({ ...form, password: e.target.value })
                            }
                            required
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-slate-700">
                            Confirm password
                        </label>
                        <input
                            type="password"
                            className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500"
                            value={form.confirmPassword}
                            onChange={e =>
                                setForm({
                                    ...form,
                                    confirmPassword: e.target.value
                                })
                            }
                            required
                        />
                    </div>

                    {error && (
                        <p className="text-sm text-red-600 text-center">
                            {error}
                        </p>
                    )}

                    <button
                        type="submit"
                        disabled={loading}
                        className="w-full rounded-lg bg-indigo-600 py-2.5 text-sm font-semibold text-white hover:bg-indigo-700 transition disabled:opacity-60"
                    >
                        Create account
                    </button>
                </form>

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
