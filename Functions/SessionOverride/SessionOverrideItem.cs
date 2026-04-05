using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Session;

public class SessionOverrideItem
{
    private readonly ISessionOverrideService _sessionOverrideService;

    public SessionOverrideItem(ISessionOverrideService sessionOverrideService)
    {
        _sessionOverrideService = sessionOverrideService;
    }

    [Function("SessionOverrideItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "put", "options",
            Route = "sessionoverride/{id}")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
         var log = context.GetLogger("SessionOverrideItem");

        // GET /sessionoverride/{id}
        if (req.Method == "GET")
        {
            var sessionOverride = await _sessionOverrideService.GetById(id);

            if (sessionOverride == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(sessionOverride);
            return ok;
        }

        // DELETE /sessionoverride/{id}
        if (req.Method == "DELETE")
        {
            try
            {
                await _sessionOverrideService.Delete(id);
                return await ApiResponseFactory.Success(req, "Instance", id, ActionType.Deleted);
            }
            catch (NotFoundException ex)
            {
                return await ApiResponseFactory.NotFound(req, ex.Message);
            }
        }

        // PUT /sessionoverride/{id}
        if (req.Method == "PUT")
        {
            var (data, errorResponse) = await req.ReadJsonBodyAsync<Domain.Models.SessionOverride>();

            if (errorResponse != null)
                return errorResponse;

            // OPTIONAL: Validate body ID if it exists
            if (data.Id != id)
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync(
                    "ID in request body does not match route ID."
                );
                return bad;
            }

            var ok = req.CreateResponse(HttpStatusCode.OK);

            if (data.Id == 0)
            {
                var created = await _sessionOverrideService.Create(data);
                if (created == null)
                    return req.CreateResponse(HttpStatusCode.InternalServerError);

                return await ApiResponseFactory.Success(req, "SessionOverride", created, ActionType.Created);
            } else
            {
                // Force route ID to be authoritative
                data.Id = id;

                var updated = await _sessionOverrideService.Update(data);

                if (updated == null)
                    return req.CreateResponse(HttpStatusCode.NotFound);
                
                return await ApiResponseFactory.Success(req, "SessionOverride", updated, ActionType.Updated);
            }
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
