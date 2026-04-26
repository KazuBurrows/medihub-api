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
            var s_id = await _sessionOverrideService.getIdByInstanceId(id);
            var sessionOverride = await _sessionOverrideService.GetById(s_id);

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
                var s_id = await _sessionOverrideService.getIdByInstanceId(id);
                await _sessionOverrideService.Delete(id);
                return await ApiResponseFactory.Success(req, "Session Override", id, ActionType.Deleted);
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

            var s_id = await _sessionOverrideService.getIdByInstanceId(id);
            data.Id = s_id;
            if (data.Id == 0 || data.Id == null)
            {
                var created = await _sessionOverrideService.Create(id, data);
                if (created == null)
                    return req.CreateResponse(HttpStatusCode.InternalServerError);

                return await ApiResponseFactory.Success(req, "SessionOverride", created, ActionType.Created);
            } else
            {
                // Force route ID to be authoritative
                s_id = await _sessionOverrideService.getIdByInstanceId(id);
                data.Id = s_id;

                var updated = await _sessionOverrideService.Update(data);

                if (updated == null)
                    return req.CreateResponse(HttpStatusCode.NotFound);
                
                return await ApiResponseFactory.Success(req, "SessionOverride", updated, ActionType.Updated);
            }
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
