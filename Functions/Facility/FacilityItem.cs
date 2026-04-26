using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Facility;

public class FacilityItem
{
    private readonly IFacilityService _facilityService;

    public FacilityItem(IFacilityService facilityService)
    {
        _facilityService = facilityService;
    }

    [Function("FacilityItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "put", "options",
            Route = "facility/{id}")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
         var log = context.GetLogger("FacilityItem");


        if (req.Method == "GET")
        {
            var facility = await _facilityService.GetById(id);

            if (facility == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(facility);
            return ok;
        }


        if (req.Method == "DELETE")
        {
            try
            {
                await _facilityService.Delete(id);
                return await ApiResponseFactory.Success(req, "Facility", id, ActionType.Deleted);
            }
            catch (NotFoundException ex)
            {
                return await ApiResponseFactory.NotFound(req, ex.Message);
            }
        }


        if (req.Method == "PUT")
        {
            var (data, errorResponse) = await req.ReadJsonBodyAsync<Domain.Models.Facility>();

            if (errorResponse != null)
                return errorResponse;

            // OPTIONAL: Validate body ID if it exists
            if (data!.Id != 0 && data.Id != id)
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync(
                    "ID in request body does not match route ID."
                );
                return bad;
            }

            // Force route ID to be authoritative
            data.Id = id;

            var updated = await _facilityService.Update(data);

            if (updated == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

                return await ApiResponseFactory.Success<Domain.Models.Facility>(req, "Facility", updated, ActionType.Updated);
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
