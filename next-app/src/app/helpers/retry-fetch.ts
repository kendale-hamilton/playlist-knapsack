import getCookies from "./cookie-functions";

export async function fetchWithRetry(
  fetchFunc: () => Promise<Response>
): Promise<Response> {
  // The backend now handles token refresh automatically
  // No need for frontend retry logic
  return await fetchFunc();
}
