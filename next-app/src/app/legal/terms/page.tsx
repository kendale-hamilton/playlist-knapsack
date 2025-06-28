"use client"
import { Card, CardBody, CardHeader, Button } from "@heroui/react";
import { useRouter } from "next/navigation";

export default function TermsOfService() {
  const router = useRouter();

  return (
    <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full min-h-screen">
      <div className="max-w-4xl mx-auto">
        <h1 className="text-3xl font-bold mb-6 text-white">Terms of Service</h1>
        
        <Card className="bg-gray-800 border border-gray-600">
          <CardHeader className="pb-4">
            <h2 className="text-xl font-semibold text-white">Last Updated: June 28, 2025</h2>
          </CardHeader>
          <CardBody className="space-y-6 text-gray-200 leading-relaxed">
            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">1. Acceptance of Terms</h3>
              <p>By accessing and using Playlist Knapsack (&quot;the Service&quot;), you accept and agree to be bound by the terms and provision of this agreement.</p>
            </section>

            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">2. Description of Service</h3>
              <p>Playlist Knapsack is a web application that uses mathematical algorithms to create playlists of specific durations from your Spotify playlists. The service integrates with Spotify&apos;s API to access your music library and create new playlists.</p>
            </section>

            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">3. User Responsibilities</h3>
              <ul className="list-disc pl-6 space-y-2 text-gray-200">
                <li>You must have a valid Spotify account to use this service</li>
                <li>You are responsible for maintaining the confidentiality of your account information</li>
                <li>You agree not to use the service for any unlawful purpose</li>
                <li>You agree not to attempt to reverse engineer or interfere with the service</li>
              </ul>
            </section>

            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">4. Spotify Integration</h3>
              <p>This service integrates with Spotify&apos;s API. By using this service, you authorize us to access your Spotify account data, including playlists and music library, for the purpose of creating new playlists. You can revoke this access at any time through your Spotify account settings.</p>
            </section>

            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">5. Data Usage</h3>
              <p>We collect and process your Spotify data solely for the purpose of providing the playlist creation service. We do not store your music files or personal Spotify credentials. We may store playlist metadata and usage statistics to improve the service.</p>
            </section>

            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">6. Intellectual Property</h3>
              <p>The algorithm and software used in this service are the intellectual property of the developer. Spotify&apos;s content and API are the property of Spotify AB. You retain rights to playlists you create through this service.</p>
            </section>

            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">7. Limitation of Liability</h3>
              <p>The service is provided &quot;as is&quot; without warranties of any kind. We are not liable for any damages arising from the use of this service, including but not limited to data loss, service interruptions, or issues with Spotify integration.</p>
            </section>

            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">8. Service Availability</h3>
              <p>We strive to maintain service availability but do not guarantee uninterrupted access. The service may be temporarily unavailable for maintenance or updates.</p>
            </section>

            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">9. Changes to Terms</h3>
              <p>We reserve the right to modify these terms at any time. Continued use of the service after changes constitutes acceptance of the new terms.</p>
            </section>
          </CardBody>
        </Card>

        <div className="flex justify-center mt-8 gap-4">
          <Button 
            color="primary" 
            onPress={() => router.push('/legal/privacy')}
            className="px-6 py-2"
          >
            View Privacy Policy
          </Button>
          <Button 
            color="secondary" 
            onPress={() => router.push('/')}
            className="px-6 py-2"
          >
            Back to Home
          </Button>
        </div>
      </div>
    </div>
  );
} 