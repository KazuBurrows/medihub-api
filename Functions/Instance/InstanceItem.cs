using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MediHub.Functions.Instance;

public class InstanceItem
{
    private readonly IInstanceService _instanceService;

    public InstanceItem(IInstanceService instanceService)
    {
        _instanceService = instanceService;
    }

    [Function("InstanceItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "put", "options",
            Route = "instance/{id}")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
        var log = context.GetLogger("InstanceItem");

        // GET /instance/{id}
        if (req.Method == "GET")
        {
            var instance = await _instanceService.GetById(id);

            if (instance == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(instance);
            return ok;
        }

        // DELETE /instance/{id}
        if (req.Method == "DELETE")
        {
            try
            {
                await _instanceService.Delete(id);
                return await ApiResponseFactory.Success(req, "Instance", id, ActionType.Deleted);
            }
            catch (NotFoundException ex)
            {
                return await ApiResponseFactory.NotFound(req, ex.Message);
            }
        }

        // PUT /instance/{id}
        if (req.Method == "PUT")
        {
            var (input, errorResponse) = await RequestValidator.ReadAndValidateAsync<Domain.Models.Instance>(req);
            if (errorResponse != null)
                return errorResponse;

            try
            {
                log.LogInformation("Updating instance with ID {input}", input.VersionId);
                input.Id = id;
                var instance = await _instanceService.Update(input);
                return await ApiResponseFactory.Success<Domain.Models.Instance>(req, "Instance", instance, ActionType.Updated);
            }
            catch (ConflictException ex)
            {
                return await ApiResponseFactory.Conflict(req, ex.Message, ex.ConflictingIds);
            }
            catch (NotFoundException ex)
            {
                return await ApiResponseFactory.NotFound(req, ex.Message);
            }
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
