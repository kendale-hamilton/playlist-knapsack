"use client"
import { Card, CardBody, CardHeader, Button } from "@heroui/react";
import { useRouter } from "next/navigation";

export default function PrivacyPolicy() {
  const router = useRouter();

  return (
    <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full min-h-screen">
      <div className="max-w-4xl mx-auto">
        <h1 className="text-3xl font-bold mb-6 text-white">Privacy Policy</h1>
        
        <Card className="bg-gray-800 border border-gray-600">
          <CardHeader className="pb-4">
            <h2 className="text-xl font-semibold text-white">Last Updated: June 28, 2025</h2>
          </CardHeader>
          <CardBody className="space-y-6 text-gray-200 leading-relaxed">
            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">1. Information We Collect</h3>
              <p>Playlist Knapsack collects the following types of information:</p>
              <ul className="list-disc pl-6 space-y-2 mt-2 text-gray-200">
                <li><strong className="text-white">Spotify Account Data:</strong> When you connect your Spotify account, we access your playlists, music library, and profile information through Spotify's API</li>
                <li><strong className="text-white">Usage Data:</strong> We collect information about how you use our service, including which playlists you process and the duration targets you set</li>
                <li><strong className="text-white">Technical Data:</strong> We may collect technical information such as IP addresses, browser type, and device information for service optimization</li>
              </ul>
            </section>

            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">2. How We Use Your Information</h3>
              <p>We use the collected information for the following purposes:</p>
              <ul className="list-disc pl-6 space-y-2 mt-2 text-gray-200">
                <li>To provide the playlist creation service</li>
                <li>To process your playlist data and generate new playlists of specific durations</li>
                <li>To improve our algorithm and service functionality</li>
                <li>To troubleshoot technical issues</li>
                <li>To communicate with you about service updates or issues</li>
              </ul>
            </section>

            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">3. Data Storage and Security</h3>
              <p>We do not store your actual music files or Spotify credentials. We may store:</p>
              <ul className="list-disc pl-6 space-y-2 mt-2 text-gray-200">
                <li>Playlist metadata (names, track lists, durations)</li>
                <li>Usage statistics and analytics</li>
                <li>Temporary session data for service functionality</li>
              </ul>
              <p className="mt-2">We implement appropriate security measures to protect your data from unauthorized access, alteration, or disclosure.</p>
            </section>

            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">4. Data Sharing</h3>
              <p>We do not sell, trade, or otherwise transfer your personal information to third parties, except:</p>
              <ul className="list-disc pl-6 space-y-2 mt-2 text-gray-200">
                <li>To comply with legal obligations</li>
                <li>To protect our rights and safety</li>
                <li>With your explicit consent</li>
                <li>To Spotify, as required for API integration</li>
              </ul>
            </section>

            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">5. Your Rights</h3>
              <p>You have the following rights regarding your data:</p>
              <ul className="list-disc pl-6 space-y-2 mt-2 text-gray-200">
                <li><strong className="text-white">Access:</strong> You can request information about what data we have about you</li>
                <li><strong className="text-white">Deletion:</strong> You can request deletion of your data from our systems</li>
                <li><strong className="text-white">Revocation:</strong> You can revoke Spotify access through your Spotify account settings</li>
                <li><strong className="text-white">Correction:</strong> You can request correction of inaccurate data</li>
              </ul>
            </section>

            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">6. Cookies and Tracking</h3>
              <p>We use cookies and similar technologies to:</p>
              <ul className="list-disc pl-6 space-y-2 mt-2 text-gray-200">
                <li>Maintain your session and authentication status</li>
                <li>Remember your preferences</li>
                <li>Analyze service usage and performance</li>
              </ul>
              <p className="mt-2">You can control cookie settings through your browser preferences.</p>
            </section>

            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">7. Third-Party Services</h3>
              <p>Our service integrates with Spotify's API. Please review Spotify's privacy policy to understand how they handle your data. We are not responsible for Spotify's data practices.</p>
            </section>

            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">8. Data Retention</h3>
              <p>We retain your data only as long as necessary to provide our services and comply with legal obligations. You can request deletion of your data at any time.</p>
            </section>

            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">9. Children's Privacy</h3>
              <p>Our service is not intended for children under 13. We do not knowingly collect personal information from children under 13. If you believe we have collected such information, please contact us immediately.</p>
            </section>

            <section>
              <h3 className="text-lg font-semibold mb-3 text-white">10. Changes to This Policy</h3>
              <p>We may update this privacy policy from time to time. We will notify you of any material changes by posting the new policy on this page and updating the "Last Updated" date.</p>
            </section>
          </CardBody>
        </Card>

        <div className="flex justify-center mt-8 gap-4">
          <Button 
            color="primary" 
            onPress={() => router.push('/legal/terms')}
            className="px-6 py-2"
          >
            View Terms of Service
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