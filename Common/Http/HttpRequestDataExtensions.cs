using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Helpers
{
    public static class HttpRequestDataExtensions
    {
        public static async Task<(T? Data, HttpResponseData? ErrorResponse)> ReadJsonBodyAsync<T>(
            this HttpRequestData req)
            where T : class
        {
            try
            {
                var data = await req.ReadFromJsonAsync<T>();
                if (data == null)
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteStringAsync("Invalid JSON data.");
                    return (null, badResponse);
                }

                return (data, null);
            }
            catch (JsonException)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Malformed JSON.");
                return (null, badResponse);
            }
        }
    }

}
