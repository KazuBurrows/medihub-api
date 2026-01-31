using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Session;

public class SessionCollectionFunction
{
    private readonly ISessionService _sessionService;

    public SessionCollectionFunction(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [Function("SessionCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "post", "options",
            Route = "session")] HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("SessionCollection");

        // POST /session
        if (req.Method == "POST")
        {
            var (data, errorResponse) =
                await req.ReadJsonBodyAsync<Domain.Models.Session>();

            if (errorResponse != null)
                return errorResponse;

            var created = await _sessionService.Create(data!);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(created);
            return response;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
