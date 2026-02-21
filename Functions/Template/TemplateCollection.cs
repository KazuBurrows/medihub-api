using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Template;

public class TemplateCollection
{
    private readonly ITemplateService _templateService;

    public TemplateCollection(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    [Function("TemplateCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "templates")] HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("TemplateCollection");

        // GET /asset
        // if (req.Method == "GET")
        // {
        //     var asset = await _templateService.GetAll();

        //     var ok = req.CreateResponse(HttpStatusCode.OK);
        //     await ok.WriteAsJsonAsync(asset);
        //     return ok;
        // }

        // POST /template
        if (req.Method == "POST")
        {
            var (data, errorResponse) =
                await req.ReadJsonBodyAsync<Domain.Models.Template>();

            if (errorResponse != null)
                return errorResponse;

            var created = await _templateService.Create(data!);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(created);
            return response;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
