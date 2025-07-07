"use client";
import { useState } from "react";
import { useRouter } from "next/navigation";
import { Card, CardBody, Button, Input } from "@heroui/react";
import { supabase } from "@/lib/supabase";
import { useAuth } from "@/hooks/useAuth";

export default function Login() {
  const { refetch } = useAuth();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const router = useRouter();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");

    try {
      const { error: authError } = await supabase.auth.signInWithPassword({
        email,
        password,
      });

      if (authError) {
        setError(authError.message);
      } else {
        await refetch();
        // Redirect to dashboard or Spotify connection
        router.push("/dashboard");
      }
    } catch (error) {
      console.error("Error during login:", error);
      setError("An unexpected error occurred");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full items-center justify-center">
      <Card className="bg-gray-800 max-w-md w-full border border-gray-600">
        <CardBody className="space-y-6 p-8">
          <h1 className="text-2xl font-bold text-center text-white mb-4">
            Sign In
          </h1>

          {error && (
            <div className="bg-red-500 text-white p-3 rounded-lg text-sm">
              {error}
            </div>
          )}

          <form onSubmit={handleLogin} className="space-y-4">
            <Input
              type="email"
              label="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              className="text-white"
              classNames={{
                input: "text-white",
                label: "text-gray-300",
              }}
            />

            <Input
              type="password"
              label="Password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              className="text-white"
              classNames={{
                input: "text-white",
                label: "text-gray-300",
              }}
            />

            <Button
              type="submit"
              color="primary"
              isLoading={loading}
              className="w-full text-white font-semibold py-3"
            >
              Sign In
            </Button>
          </form>

          <div className="text-center">
            <p className="text-gray-300">
              Don&apos;t have an account?{" "}
              <button
                onClick={() => router.push("/auth/signup")}
                className="text-blue-400 hover:text-blue-300 underline"
              >
                Sign Up
              </button>
            </p>
          </div>
        </CardBody>
      </Card>
    </div>
  );
}
