using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Theatre;

public class TheatreDTOItemFunction
{
    private readonly ITheatreService _theatreService;

    public TheatreDTOItemFunction(ITheatreService theatreService)
    {
        _theatreService = theatreService;
    }

    [Function("TheatreDTOItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "theatre/details/{id}")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
        var log = context.GetLogger("TheatreDTOItem");


        // GET /theatre/details/{id}
        if (req.Method == "GET")
        {
            var theatre = await _theatreService.GetByIdDTO(id);

            if (theatre == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(theatre);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
