using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Facility;

public class FacilityCollection
{
    private readonly IFacilityService _facilityService;

    public FacilityCollection(IFacilityService facilityService)
    {
        _facilityService = facilityService;
    }

    [Function("FacilityCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "facilities")] HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("FacilityCollection");

        // GET /facility
        if (req.Method == "GET")
        {
            var facility = await _facilityService.GetAll();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(facility);
            return ok;
        }

        // POST /facility
        if (req.Method == "POST")
        {
            var (data, errorResponse) =
                await req.ReadJsonBodyAsync<Domain.Models.Facility>();

            if (errorResponse != null)
                return errorResponse;

            var created = await _facilityService.Create(data!);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(created);
            return response;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
