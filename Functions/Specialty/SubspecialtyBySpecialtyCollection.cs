using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Specialty;

public class SubspecialtyBySpecialtyCollection
{
    private readonly ISpecialtyService _specialtyService;

    public SubspecialtyBySpecialtyCollection(ISpecialtyService specialtyService)
    {
        _specialtyService = specialtyService;
    }

    [Function("SubspecialtyBySpecialtyCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "specialties/{id}/subspecialties")] HttpRequestData req,
            int id,
        FunctionContext context)
    {
        var log = context.GetLogger("SubspecialtyBySpecialtyCollection");

        if (req.Method == "GET")
        {
            var subspecialties = await _specialtyService.GetSubspecialtiesBySpecialty(id);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(subspecialties);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
