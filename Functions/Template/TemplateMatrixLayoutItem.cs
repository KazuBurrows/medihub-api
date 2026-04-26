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

namespace MediHub.Functions.Template;

public class TemplateMatrixLayoutItem
{
    private readonly ITemplateService _templateService;

    public TemplateMatrixLayoutItem(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    [Function("TemplateMatrixLayoutItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get","options",
                    Route = "template/matrix/{view}")] HttpRequestData req,
        string view,
        FunctionContext context)
    {
        var log = context.GetLogger("TemplateMatrixLayoutItem");

        // GET
        if (req.Method == "GET")
        {
            var layout = await _templateService.GetMatrixLayout();

            if (layout == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(layout);
            return ok;
        }


        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
