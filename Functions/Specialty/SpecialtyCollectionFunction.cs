using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Specialty;

public class SpecialtyCollectionFunction
{
    private readonly ISpecialtyService _specialtyService;

    public SpecialtyCollectionFunction(ISpecialtyService specialtyService)
    {
        _specialtyService = specialtyService;
    }

    [Function("SpecialtyCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "specialty")] HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("SpecialtyCollection");

        // GET /specialty
        if (req.Method == "GET")
        {
            var specialty = await _specialtyService.GetAll();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(specialty);
            return ok;
        }

        // POST /specialty
        if (req.Method == "POST")
        {
            var (data, errorResponse) =
                await req.ReadJsonBodyAsync<Domain.Models.Specialty>();

            if (errorResponse != null)
                return errorResponse;

            var created = await _specialtyService.Create(data!);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(created);
            return response;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
