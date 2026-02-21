using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Staff;

public class StaffDTOItem
{
    private readonly IStaffService _staffService;

    public StaffDTOItem(IStaffService staffService)
    {
        _staffService = staffService;
    }

    [Function("StaffDTOItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "staff/{id}/detail")] HttpRequestData req,
        string id,
        FunctionContext context)
    {
         var log = context.GetLogger("StaffDTOItem");

        // Validate ID safely
        if (!int.TryParse(id, out var staffId))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid staff id.");
            return bad;
        }

        // GET /staff/{id}
        if (req.Method == "GET")
        {
            var staff = await _staffService.GetByIdDTO(staffId);

            if (staff == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(staff);
            return ok;
        }


        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
