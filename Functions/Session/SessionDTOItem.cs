using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Session;

public class SessionDTOItem
{
    private readonly ISessionService _sessionService;

    public SessionDTOItem(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [Function("SessionDTOItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "session/{id}/detail")] HttpRequestData req,
        string id,
        FunctionContext context)
    {
         var log = context.GetLogger("SessionDTOItem");

        // Validate ID safely
        if (!int.TryParse(id, out var sessionId))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid session id.");
            return bad;
        }

        // GET /session/{id}
        if (req.Method == "GET")
        {
            var session = await _sessionService.GetByIdDTO(sessionId);

            if (session == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(session);
            return ok;
        }


        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
