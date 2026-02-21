using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using MediHub.Infrastructure.Data.Utils;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MediHub.Functions.Staff;

public class StaffDTOCollection
{
    private readonly IStaffService _staffService;

    public StaffDTOCollection(IStaffService staffService)
    {
        _staffService = staffService;
    }

    [Function("StaffDTOCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "staffs/detail")] HttpRequestData req,
            FunctionContext context)
    {
        var log = context.GetLogger("StaffDTOCollection");

        // GET /staff
        if (req.Method == "GET")
        {
            var staff = await _staffService.GetAllDTO();
            var ok = req.CreateResponse(HttpStatusCode.OK);

            await ok.WriteAsJsonAsync(staff);
            return ok;
        }
        

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
