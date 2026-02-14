using System.Globalization;
using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MediHub.Functions.Template;

public class TemplateItemFunction
{
    private readonly ITemplateService _templateService;

    public TemplateItemFunction(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    [Function("TemplateItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "post", "options",
                    Route = "template/{dateStr}")] HttpRequestData req,
        string dateStr,
        FunctionContext context)
    {
        var log = context.GetLogger("TemplateItem");

        var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
    
        if (!bool.TryParse(query["force"], out var force))
            return await HttpResponses.BadRequest(req, "Invalid or missing force");
        
        // POST
        if (req.Method == "POST")
        {
            DateOnly date = DateOnly.Parse(dateStr, CultureInfo.InvariantCulture);

            var res = await _templateService.ApplyTemplate(date, force);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync(res);
            return response;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
