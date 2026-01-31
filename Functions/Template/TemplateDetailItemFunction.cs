using System.Globalization;
using System.Net;
using MediHub.Application.Interfaces;
using MediHub.Domain.DTOs;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Template;

public class TemplateDetailItemFunction
{
    private readonly ITemplateService _templateService;

    public TemplateDetailItemFunction(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    [Function("TemplateDetailItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "put", "post", "delete", "options",
                    Route = "template/instance/{id:int}")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
        var log = context.GetLogger("TemplateDetailItem");

        // GET
        if (req.Method == "GET")
        {
            var template = await _templateService.GetTemplateDetailDTO(id);

            if (template == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(template);
            return ok;
        }


        var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

        // Required numeric params
        if (!int.TryParse(query["sessionId"], out var sessionId))
            return await HttpResponses.BadRequest(req, "Invalid or missing sessionId");

        if (!int.TryParse(query["theatreId"], out var theatreId))
            return await HttpResponses.BadRequest(req, "Invalid or missing theatreId");

        if (!int.TryParse(query["week"], out var week))
            return await HttpResponses.BadRequest(req, "Invalid or missing week");

        if (!byte.TryParse(query["dayOfWeek"], out var dayOfWeekByte))
            return await HttpResponses.BadRequest(req, "Invalid or missing dayOfWeek");

        // Safe TimeSpan parsing
        if (!TimeSpan.TryParseExact(query["startTime"], @"hh\:mm", CultureInfo.InvariantCulture, out var startTime))
            return await HttpResponses.BadRequest(req, "Invalid or missing startTime");

        if (!TimeSpan.TryParseExact(query["endTime"], @"hh\:mm", CultureInfo.InvariantCulture, out var endTime))
            return await HttpResponses.BadRequest(req, "Invalid or missing endTime");


        // Optional staff IDs
        var staffIds = query.GetValues("staffs")?
            .SelectMany(v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(s => int.TryParse(s, out var i) ? i : (int?)null)
            .Where(i => i.HasValue)
            .Select(i => i.Value)
            .ToList()
            ?? new List<int>();

        // Call your service
        TemplateDetailDTO? instance = null;
        if (req.Method == "PUT")
        {
            instance = await _templateService.PutTemplateDetailDTO(
                id, sessionId, theatreId, week, dayOfWeekByte, startTime, endTime, staffIds);
        }
        else if (req.Method == "POST")
        {
            instance = await _templateService.CreateTemplateDetailDTO(
                sessionId, theatreId, week, dayOfWeekByte, startTime, endTime, staffIds);
        }

        if (instance == null)
            return req.CreateResponse(HttpStatusCode.NotFound);



        // DELETE
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
