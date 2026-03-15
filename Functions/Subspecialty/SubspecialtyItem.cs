using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Subspecialty;

public class SubspecialtyItem
{
    private readonly ISubspecialtyService _subspecialtyService;

    public SubspecialtyItem(ISubspecialtyService subspecialtyService)
    {
        _subspecialtyService = subspecialtyService;
    }

    [Function("SubspecialtyItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "put", "options",
            Route = "subspecialty/{id}")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
         var log = context.GetLogger("SubspecialtyItem");

        // GET /subspecialty/{id}
        if (req.Method == "GET")
        {
            var subspecialty = await _subspecialtyService.GetById(id);

            if (subspecialty == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(subspecialty);
            return ok;
        }

        // DELETE /subspecialty/{id}
        if (req.Method == "DELETE")
        {
            try
            {
                await _subspecialtyService.Delete(id);
                return await ApiResponseFactory.Success(req, "Instance", id, ActionType.Deleted);
            }
            catch (NotFoundException ex)
            {
                return await ApiResponseFactory.NotFound(req, ex.Message);
            }
        }

        // PUT /subspecialty/{id}
        if (req.Method == "PUT")
        {
            var (data, errorResponse) = await req.ReadJsonBodyAsync<Domain.Models.Subspecialty>();

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

            var updated = await _subspecialtyService.Update(data);

            if (updated == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(updated);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
