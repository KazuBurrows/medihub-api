using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Instance;

public class InstanceItemFunction
{
    private readonly IInstanceService _instanceService;

    public InstanceItemFunction(IInstanceService instanceService)
    {
        _instanceService = instanceService;
    }

    [Function("InstanceItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "put", "options",
            Route = "instance/{id}")] HttpRequestData req,
        string id,
        FunctionContext context)
    {
        var log = context.GetLogger("InstanceItem");

        // Validate ID safely
        if (!int.TryParse(id, out var instanceId))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid instance id.");
            return bad;
        }

        // GET /instance/{id}
        if (req.Method == "GET")
        {
            var instance = await _instanceService.GetById(instanceId);

            if (instance == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(instance);
            return ok;
        }

        // DELETE /instance/{id}
        if (req.Method == "DELETE")
        {
            var deleted = await _instanceService.Delete(instanceId);

            if (deleted == 0)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(deleted);
            return ok;
        }

        // PUT /instance/{id}
        if (req.Method == "PUT")
        {
            var (data, errorResponse) = await req.ReadJsonBodyAsync<Domain.Models.Instance>();

            if (errorResponse != null)
                return errorResponse;

            // OPTIONAL: Validate body ID if it exists
            if (data!.Id != 0 && data.Id != instanceId)
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync(
                    "ID in request body does not match route ID."
                );
                return bad;
            }

            // Force route ID to be authoritative
            data.Id = instanceId;

            var updated = await _instanceService.Update(data);

            if (updated == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(updated);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
