using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Staff;

public class StaffItemFunction
{
    private readonly IStaffService _staffService;

    public StaffItemFunction(IStaffService staffService)
    {
        _staffService = staffService;
    }

    [Function("StaffItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "put", "options",
            Route = "staff/{id}")] HttpRequestData req,
        string id,
        FunctionContext context)
    {
        var log = context.GetLogger("StaffItem");
        
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
            var staff = await _staffService.GetById(staffId);

            if (staff == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(staff);
            return ok;
        }

        // DELETE /staff/{id}
        if (req.Method == "DELETE")
        {
            var deleted = await _staffService.Delete(staffId);

            if (deleted == 0)
                return req.CreateResponse(HttpStatusCode.NotFound);

            return req.CreateResponse(HttpStatusCode.NoContent);
        }

        // PUT /staff/{id}
        if (req.Method == "PUT")
        {
            var (data, errorResponse) = await req.ReadJsonBodyAsync<Domain.Models.Staff>();

            if (errorResponse != null)
                return errorResponse;

            // OPTIONAL: Validate body ID if it exists
            if (data!.Id != 0 && data.Id != staffId)
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync(
                    "ID in request body does not match route ID."
                );
                return bad;
            }

            // Force route ID to be authoritative
            data.Id = staffId;

            var updated = await _staffService.Update(data);

            if (updated == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(updated);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
