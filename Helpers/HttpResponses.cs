using System.Net;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Helpers;

public static class HttpResponses
{
    public static async Task<HttpResponseData> BadRequest(HttpRequestData req, string message)
    {
        var res = req.CreateResponse(HttpStatusCode.BadRequest);
        await res.WriteStringAsync(message);
        return res;
    }

    public static async Task<HttpResponseData> Ok<T>(HttpRequestData req, T body)
    {
        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(body);
        return res;
    }

    public static HttpResponseData NotFound(HttpRequestData req)
        => req.CreateResponse(HttpStatusCode.NotFound);

    public static async Task<HttpResponseData> Conflict(HttpRequestData req, string message, object? details = null)
    {
        var res = req.CreateResponse(HttpStatusCode.Conflict);

        var body = new
        {
            message,
            details
        };

        await res.WriteAsJsonAsync(body);
        return res;
    }

}
