using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using MediHub.Infrastructure.Data.Utils;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MediHub.Functions.Session;

public class SessionDTOCollectionFunction
{
    private readonly ISessionService _sessionService;

    public SessionDTOCollectionFunction(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [Function("SessionDTOCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "session/details")] HttpRequestData req,
            FunctionContext context)
    {
        var log = context.GetLogger("SessionDTOCollection");

        // GET /session
        if (req.Method == "GET")
        {
            var session = await _sessionService.GetAll();
            var ok = req.CreateResponse(HttpStatusCode.OK);

            await ok.WriteAsJsonAsync(session);
            return ok;
        }
        

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
