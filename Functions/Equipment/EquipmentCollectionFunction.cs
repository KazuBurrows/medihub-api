using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Equipment;

public class EquipmentCollectionFunction
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentCollectionFunction(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    [Function("EquipmentCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "equipment")] HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("EquipmentCollection");


        // GET /equipment
        if (req.Method == "GET")
        {
            var equipment = await _equipmentService.GetAll();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(equipment);
            return ok;
        }

        // POST /equipment
        if (req.Method == "POST")
        {
            var (data, errorResponse) =
                await req.ReadJsonBodyAsync<Domain.Models.Equipment>();

            if (errorResponse != null)
                return errorResponse;

            var created = await _equipmentService.Create(data!);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(created);
            return response;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
