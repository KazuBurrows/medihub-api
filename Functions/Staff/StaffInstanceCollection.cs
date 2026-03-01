using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Instance;

public class StaffInstanceCollection
{
    private readonly IInstanceService _instanceService;

    public StaffInstanceCollection(IInstanceService instanceService)
    {
        _instanceService = instanceService;
    }

    [Function("StaffInstanceCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "staff/{staffId}/instances")] HttpRequestData req,
            int staffId,
        FunctionContext context)
    {
        var log = context.GetLogger("StaffInstanceCollection");

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
