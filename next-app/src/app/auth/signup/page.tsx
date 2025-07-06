"use client";
import { useState } from "react";
import { useRouter } from "next/navigation";
import { Card, CardBody, Button, Input } from "@heroui/react";
import { supabase } from "@/lib/supabase";

export default function SignUp() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const router = useRouter();

  const handleSignUp = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");

    try {
      const { data, error } = await supabase.auth.signUp({
        email,
        password,
      });

      if (error) {
        setError(error.message);
      } else {
        // Redirect to login or dashboard
        router.push(
          "/auth/login?message=Check your email to confirm your account"
        );
      }
    } catch (error) {
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
            Sign Up
          </h1>

          {error && (
            <div className="bg-red-500 text-white p-3 rounded-lg text-sm">
              {error}
            </div>
          )}

          <form onSubmit={handleSignUp} className="space-y-4">
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
              minLength={6}
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
              Sign Up
            </Button>
          </form>

          <div className="text-center">
            <p className="text-gray-300">
              Already have an account?{" "}
              <button
                onClick={() => router.push("/auth/login")}
                className="text-blue-400 hover:text-blue-300 underline"
              >
                Sign In
              </button>
            </p>
          </div>
        </CardBody>
      </Card>
    </div>
  );
}
