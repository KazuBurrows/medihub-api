using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Subspecialty;

public class SubspecialtyCollectionFunction
{
    private readonly ISubspecialtyService _subspecialtyService;

    public SubspecialtyCollectionFunction(ISubspecialtyService subspecialtyService)
    {
        _subspecialtyService = subspecialtyService;
    }

    [Function("SubspecialtyCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "subspecialty")] HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("SubspecialtyCollection");

        // GET /subspecialty
        if (req.Method == "GET")
        {
            var subspecialty = await _subspecialtyService.GetAll();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(subspecialty);
            return ok;
        }

        // POST /subspecialty
        if (req.Method == "POST")
        {
            var (data, errorResponse) =
                await req.ReadJsonBodyAsync<Domain.Models.Subspecialty>();

            if (errorResponse != null)
                return errorResponse;

            var created = await _subspecialtyService.Create(data!);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(created);
            return response;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
