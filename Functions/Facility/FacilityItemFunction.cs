using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Facility;

public class FacilityItemFunction
{
    private readonly IFacilityService _facilityService;

    public FacilityItemFunction(IFacilityService facilityService)
    {
        _facilityService = facilityService;
    }

    [Function("FacilityItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "put", "options",
            Route = "facility/{id}")] HttpRequestData req,
        string id,
        FunctionContext context)
    {
         var log = context.GetLogger("FacilityItem");

        // Validate ID safely
        if (!int.TryParse(id, out var facilityId))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid facility id.");
            return bad;
        }

        // GET /facility/{id}
        if (req.Method == "GET")
        {
            var facility = await _facilityService.GetById(facilityId);

            if (facility == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(facility);
            return ok;
        }

        // DELETE /facility/{id}
        if (req.Method == "DELETE")
        {
            var deleted = await _facilityService.Delete(facilityId);

            if (deleted == 0)
                return req.CreateResponse(HttpStatusCode.NotFound);

            return req.CreateResponse(HttpStatusCode.NoContent);
        }

        // PUT /facility/{id}
        if (req.Method == "PUT")
        {
            var (data, errorResponse) = await req.ReadJsonBodyAsync<Domain.Models.Facility>();

            if (errorResponse != null)
                return errorResponse;

            // OPTIONAL: Validate body ID if it exists
            if (data!.Id != 0 && data.Id != facilityId)
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync(
                    "ID in request body does not match route ID."
                );
                return bad;
            }

            // Force route ID to be authoritative
            data.Id = facilityId;

            var updated = await _facilityService.Update(data);

            if (updated == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(updated);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
