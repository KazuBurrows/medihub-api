using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Session;

public class SessionItem
{
    private readonly ISessionService _sessionService;

    public SessionItem(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [Function("SessionItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "put", "options",
            Route = "session/{id}")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
         var log = context.GetLogger("SessionItem");

        // GET /session/{id}
        if (req.Method == "GET")
        {
            var session = await _sessionService.GetById(id);

            if (session == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(session);
            return ok;
        }

        // DELETE /session/{id}
        if (req.Method == "DELETE")
        {
            try
            {
                await _sessionService.Delete(id);
                return await ApiResponseFactory.Success(req, "Session", id, ActionType.Deleted);
            }
            catch (NotFoundException ex)
            {
                return await ApiResponseFactory.NotFound(req, ex.Message);
            }
        }

        // PUT /session/{id}
        if (req.Method == "PUT")
        {
            var (data, errorResponse) = await req.ReadJsonBodyAsync<Domain.Models.Session>();

            if (errorResponse != null)
                return errorResponse;

            // OPTIONAL: Validate body ID if it exists
            if (data!.Id != 0 && data.Id != id)
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync(
                    "ID in request body does not match route ID."
                );
                return bad;
            }

            // Force route ID to be authoritative
            data.Id = id;

            var updated = await _sessionService.Update(data);

            if (updated == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            return await ApiResponseFactory.Success<Domain.Models.Session>(req, "Session", updated, ActionType.Updated);
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
