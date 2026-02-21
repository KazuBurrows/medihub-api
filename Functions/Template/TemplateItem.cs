using System.Globalization;
using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
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
            "get", "post", "put", "delete", "options",
                    Route = "template/{id}")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
        var log = context.GetLogger("TemplateItem");

        var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
    

        if (req.Method == "GET")
        {
            var template = await _templateService.GetById(id);

            if (template == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(template);
            return ok;
        }

        // POST
        // if (req.Method == "POST")
        // {
        //     var res = await _templateService.ApplyTemplate(id, force);

        //     var response = req.CreateResponse(HttpStatusCode.OK);
        //     await response.WriteStringAsync(res);
        //     return response;
        // }


        // PUT /session/{id}
        if (req.Method == "PUT")
        {
            var (data, errorResponse) = await req.ReadJsonBodyAsync<Domain.Models.Template>();

            if (errorResponse != null)
                return errorResponse;

            data.Id = id;
            // Log the incoming data
            log.LogInformation("Received PUT for Template ID {Id}: {Data}", id, System.Text.Json.JsonSerializer.Serialize(data));
            
            var updated = await _templateService.Update(data);

            if (updated == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(updated);
            return ok;
        }


        if (req.Method == "DELETE")
        {
            var deleted = await _templateService.Delete(id);

            if (deleted == 0)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(deleted);
            return ok;
        }


        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
