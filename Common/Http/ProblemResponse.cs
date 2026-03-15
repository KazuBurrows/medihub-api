using System.Net;
using Microsoft.Azure.Functions.Worker.Http;

public static class ProblemResponse
{
    private static async Task<HttpResponseData> Create(
        HttpRequestData req,
        HttpStatusCode status,
        string title,
        string detail,
        Dictionary<string, object>? extra = null)
    {
        var res = req.CreateResponse(status);

        await res.WriteAsJsonAsync(new ApiResponse
        {
            Title = title,
            Status = (int)status,
            Detail = detail,
            Extensions = extra
        });

        return res;
    }

    public static Task<HttpResponseData> Conflict(
        HttpRequestData req,
        string message,
        Dictionary<string, object>? extra = null)
        => Create(req, HttpStatusCode.Conflict, "Conflict", message, extra);

    public static Task<HttpResponseData> BadRequest(
        HttpRequestData req,
        string message,
        Dictionary<string, object>? extra = null)
        => Create(req, HttpStatusCode.BadRequest, "Bad Request", message, extra);

    public static Task<HttpResponseData> NotFound(
        HttpRequestData req,
        string message)
        => Create(req, HttpStatusCode.NotFound, "Not Found", message);
}