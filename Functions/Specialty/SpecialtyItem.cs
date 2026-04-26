using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Specialty;

public class SpecialtyItem
{
    private readonly ISpecialtyService _specialtyService;

    public SpecialtyItem(ISpecialtyService specialtyService)
    {
        _specialtyService = specialtyService;
    }

    [Function("SpecialtyItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "put", "options",
            Route = "specialty/{id}")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
         var log = context.GetLogger("SpecialtyItem");

        if (req.Method == "GET")
        {
            var specialty = await _specialtyService.GetById(id);

            if (specialty == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(specialty);
            return ok;
        }

        if (req.Method == "DELETE")
        {
            try
            {
                await _specialtyService.Delete(id);
                return await ApiResponseFactory.Success(req, "Specialty", id, ActionType.Deleted);
            }
            catch (NotFoundException ex)
            {
                return await ApiResponseFactory.NotFound(req, ex.Message);
            }
        }

        if (req.Method == "PUT")
        {
            var (data, errorResponse) = await req.ReadJsonBodyAsync<Domain.Models.Specialty>();

            if (errorResponse != null)
                return errorResponse;

            // OPTIONAL: Validate body ID if it exists
            if (data!.Id != 0 && data.Id != id)
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync(
                    "ID in request body does not match route ID."
                );
                return bad;
            }

            // Force route ID to be authoritative
            data.Id = id;

            var updated = await _specialtyService.Update(data);

            if (updated == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            return await ApiResponseFactory.Success<Domain.Models.Specialty>(req, "Specialty", updated, ActionType.Updated);
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
