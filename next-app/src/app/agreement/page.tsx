"use client"
import { useState, useEffect, Suspense } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { Card, CardBody, Button, Checkbox } from "@heroui/react";

function AgreementContent() {
  const [agreed, setAgreed] = useState(false);
  const router = useRouter();
  const searchParams = useSearchParams();

  useEffect(() => {
    if (typeof window !== "undefined") {
      const agreement = localStorage.getItem("playlistKnapsackAgreement");
      if (agreement === "true") {
        router.replace("/");
      }
    }
  }, [router]);

  const handleAgree = () => {
    localStorage.setItem("playlistKnapsackAgreement", "true");
    const from = searchParams.get("from");
    if (from === "signin") {
      const state = window.location.href;
      const params = new URLSearchParams({
        response_type: 'code',
        client_id: process.env.NEXT_PUBLIC_SPOTIFY_CLIENT_ID!,
        scope: 'playlist-read-private playlist-read-collaborative playlist-modify-public playlist-modify-private user-read-email ugc-image-upload',
        redirect_uri: process.env.NEXT_PUBLIC_SPOTIFY_REDIRECT_URI!,
        state: state,
        show_dialog: 'true'
      });
      window.location.href = `https://accounts.spotify.com/authorize?${params.toString()}`;
    } else {
      router.replace("/login");
    }
  };

  return (
    <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full items-center justify-center">
      <Card className="bg-gray-800 max-w-2xl w-full border border-gray-600">
        <CardBody className="space-y-6 p-8">
          <h1 className="text-3xl font-bold text-center text-white mb-4">End User Agreement Required</h1>
          <div className="text-gray-200 leading-relaxed space-y-4">
            <p className="text-lg">
              Before using Playlist Knapsack, you must review and agree to our legal documents:
            </p>
            <div className="flex flex-col sm:flex-row gap-4 justify-center">
              <a 
                href="/legal/terms" 
                className="text-blue-400 hover:text-blue-300 underline font-medium text-center px-4 py-2 bg-gray-700 rounded-lg hover:bg-gray-600 transition-colors" 
                target="_blank"
              >
                Terms of Service
              </a>
              <a 
                href="/legal/privacy" 
                className="text-blue-400 hover:text-blue-300 underline font-medium text-center px-4 py-2 bg-gray-700 rounded-lg hover:bg-gray-600 transition-colors" 
                target="_blank"
              >
                Privacy Policy
              </a>
            </div>
            <p className="text-sm text-gray-300 mt-4">
              Please read both documents carefully before proceeding. By agreeing, you acknowledge that you have read, understood, and agree to be bound by these terms.
            </p>
          </div>
          <div className="flex items-start gap-3 pt-4 border-t border-gray-600">
            <Checkbox 
              isSelected={agreed} 
              onValueChange={setAgreed} 
              id="agree-checkbox"
              classNames={{
                base: "text-white",
                label: "text-white font-medium"
              }}
            >
              <span className="text-white font-medium">
                I have read and agree to the Terms of Service and Privacy Policy
              </span>
            </Checkbox>
          </div>
          <Button 
            color="primary" 
            isDisabled={!agreed} 
            onPress={handleAgree} 
            className="w-full text-white font-semibold py-3 text-lg"
            size="lg"
          >
            I Agree and Continue
          </Button>
        </CardBody>
      </Card>
    </div>
  );
}

export default function AgreementPage() {
  return (
    <Suspense fallback={
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full items-center justify-center">
        <Card className="bg-gray-800 max-w-2xl w-full border border-gray-600">
          <CardBody className="space-y-6 p-8">
            <h1 className="text-3xl font-bold text-center text-white mb-4">Loading...</h1>
          </CardBody>
        </Card>
      </div>
    }>
      <AgreementContent />
    </Suspense>
  );
} 