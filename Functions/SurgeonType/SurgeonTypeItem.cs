using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.SurgeonType;

public class SurgeonTypeItem
{
    private readonly ISurgeonTypeService _surgeonTypeService;

    public SurgeonTypeItem(ISurgeonTypeService surgeonTypeService)
    {
        _surgeonTypeService = surgeonTypeService;
    }

    [Function("SurgeonTypeItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "put", "options",
            Route = "surgeon-type/{id}")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
         var log = context.GetLogger("SurgeonTypeItem");

        // GET /surgeon-type/{id}
        if (req.Method == "GET")
        {
            var surgeonType = await _surgeonTypeService.GetById(id);

            if (surgeonType == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(surgeonType);
            return ok;
        }

        // DELETE /surgeon-type/{id}
        if (req.Method == "DELETE")
        {
            try
            {
                await _surgeonTypeService.Delete(id);
                return await ApiResponseFactory.Success(req, "SurgeonType", id, ActionType.Deleted);
            }
            catch (NotFoundException ex)
            {
                return await ApiResponseFactory.NotFound(req, ex.Message);
            }
        }

        // PUT /surgeon-type/{id}
        if (req.Method == "PUT")
        {
            var (data, errorResponse) = await req.ReadJsonBodyAsync<Domain.Models.SurgeonType>();

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

            var updated = await _surgeonTypeService.Update(data);

            if (updated == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            return await ApiResponseFactory.Success<Domain.Models.SurgeonType>(req, "Surgeon Type", updated, ActionType.Updated);
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
