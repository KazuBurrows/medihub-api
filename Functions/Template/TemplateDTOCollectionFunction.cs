using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using MediHub.Infrastructure.Data.Utils;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MediHub.Functions.Template;

public class TemplateDTOCollectionFunction
{
    private readonly ITemplateService _templateService;

    public TemplateDTOCollectionFunction(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    [Function("TemplateDTOCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "template/list/details")] HttpRequestData req,
            FunctionContext context)
    {
        var log = context.GetLogger("TemplateDTOCollection");

        // GET /template
        if (req.Method == "GET")
        {
            var ok = req.CreateResponse(HttpStatusCode.OK);
            var templates = await _templateService.GetAllDTO();
            await ok.WriteAsJsonAsync(templates);

            return ok;
        }
        

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
