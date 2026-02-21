using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Staff;

public class StaffCollection
{
    private readonly IStaffService _staffService;

    public StaffCollection(IStaffService staffService)
    {
        _staffService = staffService;
    }

    [Function("StaffCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "staffs")] HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("StaffCollection");

        // GET /staff
        if (req.Method == "GET")
        {
            var staff = await _staffService.GetAll();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(staff);
            return ok;
        }

        // POST /staff
        if (req.Method == "POST")
        {
            var (data, errorResponse) =
                await req.ReadJsonBodyAsync<Domain.Models.Staff>();

            if (errorResponse != null)
                return errorResponse;

            var created = await _staffService.Create(data!);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(created);
            return response;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
