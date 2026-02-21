using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using MediHub.Infrastructure.Data.Utils;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MediHub.Functions.Session;

public class SessionDTOCollection
{
    private readonly ISessionService _sessionService;

    public SessionDTOCollection(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [Function("SessionDTOCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "sessions/detail")] HttpRequestData req,
            FunctionContext context)
    {
        var log = context.GetLogger("SessionDTOCollection");

        // GET /session
        if (req.Method == "GET")
        {
            var session = await _sessionService.GetAllDTO();
            var ok = req.CreateResponse(HttpStatusCode.OK);

            await ok.WriteAsJsonAsync(session);
            return ok;
        }
        

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
