using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.AnaestheticType;

public class AnaestheticTypeCollection
{
    private readonly IAnaestheticTypeService _anaestheticTypeService;

    public AnaestheticTypeCollection(IAnaestheticTypeService anaestheticTypeService)
    {
        _anaestheticTypeService = anaestheticTypeService;
    }

    [Function("AnaestheticTypeCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "anaesthetic-types")] HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("AnaestheticTypeCollection");

        // GET /anaesthetic-types
        if (req.Method == "GET")
        {
            var anaestheticType = await _anaestheticTypeService.GetAll();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(anaestheticType);
            return ok;
        }

        // POST /anaesthetic-types
        if (req.Method == "POST")
        {
            var (data, errorResponse) =
                await req.ReadJsonBodyAsync<Domain.Models.AnaestheticType>();

            if (errorResponse != null)
                return errorResponse;

            var created = await _anaestheticTypeService.Create(data!);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(created);
            return response;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
