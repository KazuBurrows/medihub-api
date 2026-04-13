using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Specialty;

public class SpecialtyDTOCollection
{
    private readonly ISpecialtyService _specialtyService;

    public SpecialtyDTOCollection(ISpecialtyService specialtyService)
    {
        _specialtyService = specialtyService;
    }

    [Function("SpecialtyDTOCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "specialties/detail")] HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("SpecialtyDTOCollection");

        if (req.Method == "GET")
        {
            var specialty = await _specialtyService.GetAllDTO();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(specialty);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
