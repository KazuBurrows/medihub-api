using System.Globalization;
using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MediHub.Functions.Template;

public class TemplateItem
{
    private readonly ITemplateService _templateService;

    public TemplateItem(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    [Function("TemplateItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "options",
                    Route = "template/{id}")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
        var log = context.GetLogger("TemplateItem");

        if (req.Method == "GET")
        {
            var template = await _templateService.GetById(id);

            if (template == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(template);
            return ok;
        }


        if (req.Method == "DELETE")
        {
            try
            {
                await _templateService.Delete(id);
                return await ApiResponseFactory.Success(req, "Template", id, ActionType.Deleted);
            }
            catch (NotFoundException ex)
            {
                return await ApiResponseFactory.NotFound(req, ex.Message);
            }
        }


        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
