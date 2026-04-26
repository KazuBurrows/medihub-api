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

public class InstanceDTOCollectionByDate
{
    private readonly IInstanceService _instanceService;

    public InstanceDTOCollectionByDate(IInstanceService instanceService)
    {
        _instanceService = instanceService;
    }

    [Function("InstanceDTOCollectionByDate")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "instances/detail/{startDate}/{endDate}/{versionId}")] HttpRequestData req,
            string startDate,
            string endDate,
            int versionId,
            FunctionContext context)
    {
        var log = context.GetLogger("InstanceDTOCollectionByDate");

        // GET /instance
        if (req.Method == "GET")
        {
            var ok = req.CreateResponse(HttpStatusCode.OK);
            
            var instances = await _instanceService.GetAllDTOByDate(startDate, endDate, versionId);

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

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
