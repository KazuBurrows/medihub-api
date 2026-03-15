using System.Net;
using System.Security.Claims;
using System.Text.Json;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Domain.DTOs;
using MediHub.Functions.Helpers;
using MediHub.Infrastructure.Data.Utils;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MediHub.Functions.Instance;

public class InstanceDTOCollection
{
    private readonly IInstanceService _instanceService;

    public InstanceDTOCollection(IInstanceService instanceService)
    {
        _instanceService = instanceService;
    }

    [Function("InstanceDTOCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "instances/detail")] HttpRequestData req,
            FunctionContext context)
    {
        var log = context.GetLogger("InstanceDTOCollection");

        // GET /instance
        if (req.Method == "GET")
        {
            var ok = req.CreateResponse(HttpStatusCode.OK);
            IEnumerable<InstanceDTO> instances;

            instances = await _instanceService.GetAllDTO();

            // Log the first 10 instances
            var first10 = instances.Take(10);
            var json = System.Text.Json.JsonSerializer.Serialize(first10, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true // optional, makes log easier to read
            });

            log.LogInformation("First 10 instances: {Instances}", json);
            await ok.WriteAsJsonAsync(instances);

            return ok;
        }

        // POST /template
        if (req.Method == "POST")
        {
            var (input, errorResponse) = await RequestValidator.ReadAndValidateAsync<InstanceDTO>(req);
            if (errorResponse != null)
                return errorResponse;

            try
            {
                var instance = await _instanceService.CreateDTO(input);
                return await ApiResponseFactory.Success<Domain.DTOs.InstanceDTO>(req, "Instance", instance, ActionType.Updated);
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
