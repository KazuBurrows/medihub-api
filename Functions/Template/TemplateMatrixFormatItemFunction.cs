using System.Globalization;
using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using MediHub.Infrastructure.Data.Utils;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MediHub.Functions.Template;

public class TemplateMatrixFormatItemFunction
{
    private readonly ITemplateService _templateService;

    public TemplateMatrixFormatItemFunction(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    [Function("TemplateMatrixFormatItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "template/matrix/format/{facilityId:int?}")]
        HttpRequestData req,
        int? facilityId,
        FunctionContext context)
    {
        var log = context.GetLogger("TemplateMatrixFormatItem");

        // --- Default values ---
        facilityId ??= 0;

        // --- GET endpoint ---
        if (req.Method == "GET")
        {
            var template = await _templateService.GetMatrixFormat((int)facilityId);

            if (template == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(template);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
