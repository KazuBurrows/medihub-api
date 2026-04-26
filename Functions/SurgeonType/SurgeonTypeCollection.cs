using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.SurgeonType;

public class SurgeonTypeCollection
{
    private readonly ISurgeonTypeService _surgeonTypeService;

    public SurgeonTypeCollection(ISurgeonTypeService surgeonTypeService)
    {
        _surgeonTypeService = surgeonTypeService;
    }

    [Function("SurgeonTypeCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "surgeon-types")] HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("SurgeonTypeCollection");

        // GET /surgeon-types
        if (req.Method == "GET")
        {
            var surgeonType = await _surgeonTypeService.GetAll();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(surgeonType);
            return ok;
        }

        // POST /surgeon-types
        if (req.Method == "POST")
        {
            var (data, errorResponse) =
                await req.ReadJsonBodyAsync<Domain.Models.SurgeonType>();

            if (errorResponse != null)
                return errorResponse;

            var created = await _surgeonTypeService.Create(data!);

            return await ApiResponseFactory.Success<Domain.Models.SurgeonType>(req, "Surgeon Type", created, ActionType.Created);
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
