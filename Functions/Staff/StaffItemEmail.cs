using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Staff;

public class StaffEmailItem
{
    private readonly IStaffService _staffService;

    public StaffEmailItem(IStaffService staffService)
    {
        _staffService = staffService;
    }

    [Function("StaffEmailItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "put", "options",
            Route = "staff/email/{email}")] HttpRequestData req,
        string email,
        FunctionContext context)
    {
        var log = context.GetLogger("StaffEmailItem");
        

        // GET /staff/{id}
        if (req.Method == "GET")
        {
            var staff = await _staffService.GetByEmail(email);

            if (staff == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(staff);
            return ok;
        }
        

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
