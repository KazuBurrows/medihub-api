using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Theatre;

public class TheatreAggItemFunction
{
    private readonly ITheatreService _theatreService;

    public TheatreAggItemFunction(ITheatreService theatreService)
    {
        _theatreService = theatreService;
    }

    [Function("TheatreAggItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "put", "options",
            Route = "theatre/{id}/aggregate")] HttpRequestData req,
        string id,
        FunctionContext context)
    {
        var log = context.GetLogger("TheatreAggItem");
        
        // Validate ID safely
        if (!int.TryParse(id, out var theatreId))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid theatre id.");
            return bad;
        }

        // GET /theatre/{id}aggregate
        if (req.Method == "GET")
        {
            var theatre = await _theatreService.GetByIdAgg(theatreId);

            if (theatre == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(theatre);
            return ok;
        }

        // DELETE /theatre/{id}/aggregate
        if (req.Method == "DELETE")
        {
            var deleted = await _theatreService.DeleteAgg(theatreId);

            if (deleted == 0)
                return req.CreateResponse(HttpStatusCode.NotFound);

            return req.CreateResponse(HttpStatusCode.NoContent);
        }

        // PUT /theatre/{id}/aggregate
        if (req.Method == "PUT")
        {
            var (data, errorResponse) = await req.ReadJsonBodyAsync<Domain.DTOs.TheatreAggregate>();

            if (errorResponse != null)
                return errorResponse;

            // OPTIONAL: Validate body ID if it exists
            if (data!.Id != 0 && data.Id != theatreId)
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync(
                    "ID in request body does not match route ID."
                );
                return bad;
            }

            // Force route ID to be authoritative
            data.Id = theatreId;

            var updated = await _theatreService.UpdateAgg(data);

            if (updated == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(updated);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
