using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Instance;

public class InstanceMatrixCollection
{
    private readonly IInstanceService _instanceService;

    public InstanceMatrixCollection(IInstanceService instanceService)
    {
        _instanceService = instanceService;
    }

    [Function("InstanceMatrixCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "instances/matrix/{view}/{date}/{versionId}")]
        HttpRequestData req,
        string view,
        string date,
        int versionId,
        FunctionContext context)
    {
        var log = context.GetLogger("InstanceMatrixCollection");

        // GET /instance
        if (req.Method == "GET")
        {
            DateTime dt = DateTime.Parse(date);
            DateOnly dateOnly = DateOnly.FromDateTime(dt);
            var instance = await _instanceService.GetAllWeekMatrix(dateOnly, versionId);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(instance);
            return ok;
        }
        

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
