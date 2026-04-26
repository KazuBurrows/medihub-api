using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Equipment;

public class EquipmentItem
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentItem(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    [Function("EquipmentItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "put", "options",
            Route = "equipment/{id}")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
         var log = context.GetLogger("EquipmentItem");

        // GET /equipment/{id}
        if (req.Method == "GET")
        {
            var equipment = await _equipmentService.GetById(id);

            if (equipment == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(equipment);
            return ok;
        }

        // DELETE /equipment/{id}
        if (req.Method == "DELETE")
        {
            try
            {
                await _equipmentService.Delete(id);
                return await ApiResponseFactory.Success(req, "Equipment", id, ActionType.Deleted);
            }
            catch (NotFoundException ex)
            {
                return await ApiResponseFactory.NotFound(req, ex.Message);
            }
        }

        // PUT /equipment/{id}
        if (req.Method == "PUT")
        {
            var (data, errorResponse) = await req.ReadJsonBodyAsync<Domain.Models.Equipment>();

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

            var updated = await _equipmentService.Update(data);

            if (updated == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

                return await ApiResponseFactory.Success<Domain.Models.Equipment>(req, "Equipment", updated, ActionType.Updated);
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
