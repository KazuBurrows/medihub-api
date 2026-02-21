using System.Globalization;
using System.Net;
using MediHub.Application.Interfaces;
using MediHub.Domain.DTOs;
using MediHub.Functions.Helpers;
using MediHub.Functions.Helpers.Exceptions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MediHub.Functions.Instance;

public class InstanceDTOItem
{
    private readonly IInstanceService _instanceService;

    public InstanceDTOItem(IInstanceService instanceService)
    {
        _instanceService = instanceService;
    }

    [Function("InstanceDTOItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "put", "delete","options",
                    Route = "instance/{id:int}/detail")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
        var log = context.GetLogger("InstanceDTOItem");

        // GET
        if (req.Method == "GET")
        {
            var instance = await _instanceService.GetByIdDTO(id);

            if (instance == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(instance);
            return ok;
        }

        // PUT
        if (req.Method == "PUT")
        {
            try
            {
                // Parse the request body as InstanceDTO
                var data = await req.ReadFromJsonAsync<InstanceDTO>();
                if (data == null)
                {
                    log.LogWarning($"PUT request body for instance {id} was null or invalid.");
                    return req.CreateResponse(HttpStatusCode.BadRequest);
                }

                // Log the incoming data
                log.LogInformation($"PUT request for instance {id} received: {System.Text.Json.JsonSerializer.Serialize(data)}");

                // Ensure the ID from the route matches the DTO ID (optional, for safety)
                data.Id = id;

                // Update the instance via your service
                var updatedInstance = await _instanceService.UpdateDTO(data);

                if (updatedInstance == null)
                    return req.CreateResponse(HttpStatusCode.NotFound);

                // Return the updated instance
                var ok = req.CreateResponse(HttpStatusCode.OK);
                await ok.WriteAsJsonAsync(updatedInstance);
                return ok;
            }
            catch (Exception ex)
            {
                log.LogError($"Error updating instance {id}: {ex.Message}");
                var errorResp = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResp.WriteStringAsync($"Error updating instance: {ex.Message}");
                return errorResp;
            }
        }

        // DELETE
        if (req.Method == "DELETE")
        {
            var deleted = await _instanceService.DeleteDTO(id);

            if (deleted == 0)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(deleted);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
