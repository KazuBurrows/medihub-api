using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Theatre;

public class TheatreCollectionFunction
{
    private readonly ITheatreService _theatreService;

    public TheatreCollectionFunction(ITheatreService theatreService)
    {
        _theatreService = theatreService;
    }

    [Function("TheatreCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "theatre")] HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("TheatreCollection");

        // GET /theatre
        if (req.Method == "GET")
        {
            var theatre = await _theatreService.GetAll();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(theatre);
            return ok;
        }

        // POST /theatre
        if (req.Method == "POST")
        {
            var (data, errorResponse) =
                await req.ReadJsonBodyAsync<Domain.Models.Theatre>();

            if (errorResponse != null)
                return errorResponse;

            var created = await _theatreService.Create(data!);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(created);
            return response;
        }
        

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
