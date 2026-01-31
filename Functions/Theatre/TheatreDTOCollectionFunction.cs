using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Theatre;

public class TheatreDTOCollectionFunction
{
    private readonly ITheatreService _theatreService;

    public TheatreDTOCollectionFunction(ITheatreService theatreService)
    {
        _theatreService = theatreService;
    }

    [Function("TheatreDTOCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "theatre/details")] HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("TheatreDTOCollection");

        // GET /theatre
        if (req.Method == "GET")
        {
            var theatre = await _theatreService.GetAllDTO();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(theatre);
            return ok;
        }
        

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
