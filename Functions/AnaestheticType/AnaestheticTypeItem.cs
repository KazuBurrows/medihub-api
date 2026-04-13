using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.AnaestheticType;

public class AnaestheticTypeItem
{
    private readonly IAnaestheticTypeService _anaestheticTypeService;

    public AnaestheticTypeItem(IAnaestheticTypeService anaestheticTypeService)
    {
        _anaestheticTypeService = anaestheticTypeService;
    }

    [Function("AnaestheticTypeItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "put", "options",
            Route = "anaesthetic-type/{id}")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
         var log = context.GetLogger("AnaestheticTypeItem");

        // GET /anaesthetic-type/{id}
        if (req.Method == "GET")
        {
            var anaestheticType = await _anaestheticTypeService.GetById(id);

            if (anaestheticType == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(anaestheticType);
            return ok;
        }

        // DELETE /anaesthetic-type/{id}
        if (req.Method == "DELETE")
        {
            try
            {
                await _anaestheticTypeService.Delete(id);
                return await ApiResponseFactory.Success(req, "AnaestheticType", id, ActionType.Deleted);
            }
            catch (NotFoundException ex)
            {
                return await ApiResponseFactory.NotFound(req, ex.Message);
            }
        }

        // PUT /anaesthetic-type/{id}
        if (req.Method == "PUT")
        {
            var (data, errorResponse) = await req.ReadJsonBodyAsync<Domain.Models.AnaestheticType>();

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

            var updated = await _anaestheticTypeService.Update(data);

            if (updated == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(updated);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
