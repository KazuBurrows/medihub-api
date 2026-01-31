using System.Net;
using System.Security.Claims;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MediHub.Functions
{
    public static class AuthHelper
    {
        /// <summary>
        /// Validates the request including CORS preflight and token.
        /// Returns the ClaimsPrincipal if valid.
        /// If the request is OPTIONS, it returns null and sets response accordingly.
        /// Throws nothing — just returns null if unauthorized.
        /// </summary>
        public class HttpResponseException : Exception
        {
            public HttpResponseData Response { get; }
            public HttpResponseException(HttpResponseData response) => Response = response;
        }

        /// <summary>
        /// Validates request, handles preflight and token.
        /// Returns ClaimsPrincipal if valid; throws HttpResponseException otherwise.
        /// </summary>
        public static async Task<(ClaimsPrincipal, string)> CheckRequestAsync(
            HttpRequestData req,
            ILogger log,
            string tenantId,
            string audience)
        {
            // 1️⃣ Handle OPTIONS preflight
            if (req.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                var preflight = req.CreateResponse(HttpStatusCode.NoContent);
                preflight.Headers.Add("Access-Control-Allow-Origin", "http://localhost:3000");
                preflight.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                preflight.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                throw new HttpResponseException(preflight);
            }

            // 2️⃣ Always add CORS header to other responses
            req.CreateResponse().Headers.Add("Access-Control-Allow-Origin", "http://localhost:3000");

            // 3️⃣ Validate Authorization header
            if (!req.Headers.TryGetValues("Authorization", out var authHeaders))
            {
                log.LogWarning("Authorization header missing.");
                var unauthorizedResp = req.CreateResponse(HttpStatusCode.Unauthorized);
                await unauthorizedResp.WriteStringAsync("Missing Authorization header");
                throw new HttpResponseException(unauthorizedResp);
            }

            var token = authHeaders.First().Replace("Bearer ", "");
            log.LogInformation($"Token received: {token.Substring(0, Math.Min(token.Length, 20))}...");

            // 4️⃣ Validate token
            ClaimsPrincipal? user;
            try
            {
                user = TokenValidator.ValidateToken(token, tenantId, audience);
            }
            catch (Exception ex)
            {
                log.LogError($"Token validation exception: {ex.Message}");
                var unauthorizedResp = req.CreateResponse(HttpStatusCode.Unauthorized);
                await unauthorizedResp.WriteStringAsync("Error validating token");
                throw new HttpResponseException(unauthorizedResp);
            }

            if (user == null)
            {
                log.LogWarning("Token invalid.");
                var unauthorizedResp = req.CreateResponse(HttpStatusCode.Unauthorized);
                await unauthorizedResp.WriteStringAsync("Invalid token");
                throw new HttpResponseException(unauthorizedResp);
            }

            // ✅ Token valid
            log.LogInformation("Token validated successfully.");
            return (user, token);
        }
    }
}
