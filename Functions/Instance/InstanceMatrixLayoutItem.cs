using System.Globalization;
using System.Net;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Domain.DTOs;
using MediHub.Functions.Helpers;
using MediHub.Functions.Helpers.Exceptions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MediHub.Functions.Instance;

public class InstanceMatrixLayoutItem
{
    private readonly IInstanceService _instanceService;

    public InstanceMatrixLayoutItem(IInstanceService instanceService)
    {
        _instanceService = instanceService;
    }

    [Function("InstanceMatrixLayoutItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get","options",
                    Route = "instance/matrix/{view}")] HttpRequestData req,
        string view,
        FunctionContext context)
    {
        var log = context.GetLogger("InstanceMatrixLayoutItem");

        // GET
        if (req.Method == "GET")
        {
            var layout = await _instanceService.GetMatrixLayout();

            if (layout == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(layout);
            return ok;
        }


        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
