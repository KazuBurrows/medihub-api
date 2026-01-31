using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Specialty;

public class SpecialtyItemFunction
{
    private readonly ISpecialtyService _specialtyService;

    public SpecialtyItemFunction(ISpecialtyService specialtyService)
    {
        _specialtyService = specialtyService;
    }

    [Function("SpecialtyItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "put", "options",
            Route = "specialty/{id}")] HttpRequestData req,
        string id,
        FunctionContext context)
    {
         var log = context.GetLogger("SpecialtyItem");
            
        // Validate ID safely
        if (!int.TryParse(id, out var specialtyId))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid specialty id.");
            return bad;
        }

        // GET /specialty/{id}
        if (req.Method == "GET")
        {
            var specialty = await _specialtyService.GetById(specialtyId);

            if (specialty == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(specialty);
            return ok;
        }

        // DELETE /specialty/{id}
        if (req.Method == "DELETE")
        {
            var deleted = await _specialtyService.Delete(specialtyId);

            if (deleted == 0)
                return req.CreateResponse(HttpStatusCode.NotFound);

            return req.CreateResponse(HttpStatusCode.NoContent);
        }

        // PUT /specialty/{id}
        if (req.Method == "PUT")
        {
            var (data, errorResponse) = await req.ReadJsonBodyAsync<Domain.Models.Specialty>();

            if (errorResponse != null)
                return errorResponse;

            // OPTIONAL: Validate body ID if it exists
            if (data!.Id != 0 && data.Id != specialtyId)
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync(
                    "ID in request body does not match route ID."
                );
                return bad;
            }

            // Force route ID to be authoritative
            data.Id = specialtyId;

            var updated = await _specialtyService.Update(data);

            if (updated == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(updated);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
