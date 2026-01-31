using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Theatre;

public class TheatreAggCollectionFunction
{
    private readonly ITheatreService _theatreService;

    public TheatreAggCollectionFunction(ITheatreService theatreService)
    {
        _theatreService = theatreService;
    }

    [Function("TheatreAggCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "theatre/aggregate")] HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("TheatreAggCollection");

        // GET /theatre/aggregate
        if (req.Method == "GET")
        {
            var theatre = await _theatreService.GetAllAgg();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(theatre);
            return ok;
        }

        // POST /theatre/aggregate
        if (req.Method == "POST")
        {
            var (data, errorResponse) =
                await req.ReadJsonBodyAsync<Domain.DTOs.TheatreAggregate>();

            if (errorResponse != null)
                return errorResponse;

            var created = await _theatreService.CreateAgg(data!);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(created);
            return response;
        }
        

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
