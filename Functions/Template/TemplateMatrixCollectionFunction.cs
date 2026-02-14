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

public class TemplateMatrixCollectionFunction
{
    private readonly ITemplateService _templateService;

    public TemplateMatrixCollectionFunction(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    [Function("TemplateMatrixCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "template/matrix/{week:int?}/{facility:int?}/{asset:int?}")]
        HttpRequestData req,
        int? week,
        int? facility,
        int? asset,
        FunctionContext context)
    {
        var log = context.GetLogger("TemplateMatrixCollection");

        // facility/asset null means "all"
        int? facilityId = facility.HasValue && facility.Value > 0 ? facility.Value : null;
        int? assetId = asset.HasValue && asset.Value > 0 ? asset.Value : null;
        week ??= 1;

        // --- GET endpoint ---
        if (req.Method == "GET")
        {
            var template = await _templateService.GetMatrix(week.Value, facilityId, assetId);

            if (template == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(template);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
