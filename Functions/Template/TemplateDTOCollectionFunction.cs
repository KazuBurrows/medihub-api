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

    // [Function("TemplateDTOCollection")]
    // public async Task<HttpResponseData> Run(
    //     [HttpTrigger(
    //         AuthorizationLevel.Anonymous,
    //         "get", "options",
    //         Route = "template/details/{year:int?}/{week:int?}")] HttpRequestData req,
    //         int? year,
    //         int? week,
    //         FunctionContext context)
    // {
    //     var log = context.GetLogger("TemplateDTOCollection");

    //     if (!year.HasValue)
    //     {
    //         year = DateTime.UtcNow.Year;
    //     }

    //     // GET /template
    //     if (req.Method == "GET")
    //     {
    //         var ok = req.CreateResponse(HttpStatusCode.OK);
    //         if (!week.HasValue)
    //         {
    //             // return all templates for the year
    //             var template = await _templateService.GetAllDTO(year.Value);
    //             await ok.WriteAsJsonAsync(template);
    //         } else
    //         {
    //             var template = await _templateService.GetMatrixByWeek(year.Value, week.Value);
    //             await ok.WriteAsJsonAsync(template);
    //         }

    //         return ok;
    //     }
        

    //     return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    // }
}
