using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Instance;

public class InstanceCollection
{
    private readonly IInstanceService _instanceService;

    public InstanceCollection(IInstanceService instanceService)
    {
        _instanceService = instanceService;
    }

    [Function("InstanceCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "instances")] HttpRequestData req,
        FunctionContext context)
    { 
        var log = context.GetLogger("InstanceCollection");

        // GET /instance
        if (req.Method == "GET")
        {
            var instance = await _instanceService.GetAll();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(instance);
            return ok;
        }

        // POST /instance
        if (req.Method == "POST")
        {
            var (data, errorResponse) =
                await req.ReadJsonBodyAsync<Domain.Models.Instance>();

            if (errorResponse != null)
                return errorResponse;

            var created = await _instanceService.Create(data!);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(created);
            return response;
        }
        

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
