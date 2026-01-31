using System.Net;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using MediHub.Functions.Helpers.Exceptions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Schedule;

public class InstanceDetailItemFunction
{
    private readonly IScheduleService _scheduleService;

    public InstanceDetailItemFunction(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [Function("InstanceDetailItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "put", "post", "options",
                    Route = "schedule/instance/{id:int}")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
        var log = context.GetLogger("InstanceDetailItem");

        // GET /schedule/instance/{id:int}
        if (req.Method == "GET")
        {
            var schedule = await _scheduleService.GetInstanceDetailDTO(id);

            if (schedule == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(schedule);
            return ok;
        }


        // Parse parameters from request
        var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

        if (!int.TryParse(query["sessionId"], out var sessionId))
            return await HttpResponses.BadRequest(req, "Invalid or missing sessionId");

        if (!int.TryParse(query["theatreId"], out var theatreId))
            return await HttpResponses.BadRequest(req, "Invalid or missing theatreId");

        var startDatetime = query["startDatetime"];
        if (string.IsNullOrWhiteSpace(startDatetime))
            return await HttpResponses.BadRequest(req, "Missing startDatetime");

        var endDatetime = query["endDatetime"];
        if (string.IsNullOrWhiteSpace(endDatetime))
            return await HttpResponses.BadRequest(req, "Missing endDatetime");


        var staffIds = query.GetValues("staffs")?
            .SelectMany(v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(s => int.TryParse(s, out var i) ? i : (int?)null)
            .Where(i => i.HasValue)
            .Select(i => i.Value)
            .ToList()
            ?? new List<int>();

        if (!bool.TryParse(query["force"], out var force))
            return await HttpResponses.BadRequest(req, "Invalid or missing force");


        if (req.Method == "PUT")
        {
            var instance = await _scheduleService.PutInstanceDetailDTO(id, (int)sessionId, (int)theatreId, startDatetime, endDatetime, staffIds, force);

            if (instance == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(instance);
            return ok;
        }


        if (req.Method == "POST")
        {
            try
            {
                var instance = await _scheduleService.CreateInstanceDetailDTO((int)sessionId, (int)theatreId, startDatetime, endDatetime, staffIds, force);
                if (instance == null)
                    return req.CreateResponse(HttpStatusCode.NotFound);

                var ok = req.CreateResponse(HttpStatusCode.OK);
                await ok.WriteAsJsonAsync(instance);
                return ok;
            }
            catch (InstanceClashException ex)
            {
                return await HttpResponses.Conflict(req, ex.Message);
            }

        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
