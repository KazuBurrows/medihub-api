using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Domain.DTOs;
using MediHub.Functions.Helpers;
using MediHub.Functions.Helpers.Exceptions;
using MediHub.Infrastructure.Data.Utils;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MediHub.Functions.Template;

public class TemplateDTOCollectionByWeek
{
    private readonly ITemplateService _templateService;

    public TemplateDTOCollectionByWeek(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    [Function("TemplateDTOCollectionByWeek")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "templates/detail/{week}/{versionId}")] HttpRequestData req,
            int week,
            int versionId,
            FunctionContext context)
    {
        var log = context.GetLogger("TemplateDTOCollection");

        // GET /template
        if (req.Method == "GET")
        {
            var ok = req.CreateResponse(HttpStatusCode.OK);
            IEnumerable<TemplateDTO> templates;

            templates = await _templateService.GetAllDTOByWeek(week, versionId);
            await ok.WriteAsJsonAsync(templates);

            return ok;
        }


        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
