using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Instance;

public class InstanceStaffCollectionFunction
{
    private readonly IInstanceService _instanceService;

    public InstanceStaffCollectionFunction(IInstanceService instanceService)
    {
        _instanceService = instanceService;
    }

    [Function("InstanceStaffCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "instance/staff/{staffId}")] HttpRequestData req,
            int staffId,
        FunctionContext context)
    {
        var log = context.GetLogger("InstanceCollection");

        // GET /instance
        if (req.Method == "GET")
        {
            var instance = await _instanceService.GetAllByStaffId(staffId);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(instance);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
