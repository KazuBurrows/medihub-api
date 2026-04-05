using System.Globalization;
using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MediHub.Functions.Template;

public class TemplateByDateCollection
{
    private readonly ITemplateService _templateService;

    public TemplateByDateCollection(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    [Function("TemplateByDateCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "post", "options",
                    Route = "templates/apply/{date}/{cycleWeek}")] HttpRequestData req,
        string date,
        int cycleWeek,
        FunctionContext context)
    {
        var log = context.GetLogger("TemplateByDateCollection");

        // var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
    
        // POST
        if (req.Method == "POST")
        {
            DateTime dateTime = DateTime.Parse(date);
            DateOnly dateOnly = DateOnly.FromDateTime(dateTime);
            var res = await _templateService.ApplyTemplate(dateOnly, cycleWeek);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(res);
            return response;
        }


        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
