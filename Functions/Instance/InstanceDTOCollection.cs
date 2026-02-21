using System.Net;
using System.Security.Claims;
using System.Text.Json;
using MediHub.Application.Interfaces;
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
            var instances = await _instanceService.GetAllDTO();

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
            log.LogInformation("POST /instance endpoint hit");

            // Read raw body ONCE
            var rawBody = await new StreamReader(req.Body).ReadToEndAsync();

            log.LogInformation("Raw request body: {Body}", rawBody);

            // Deserialize manually
            var data = JsonSerializer.Deserialize<InstanceDTO>(
                rawBody,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (data == null)
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync("Invalid JSON");
                return bad;
            }

            log.LogInformation("InstanceDTO received: {@Instance}", data);

            var created = await _instanceService.CreateDTO(data);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(created);

            return response;
        }


        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
