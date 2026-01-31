using System.Net;
using MediHub.Application.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Schedule;

public class ListCollectionFunction
{
    private readonly IScheduleService _scheduleService;

    public ListCollectionFunction(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [Function("ListCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "schedule/list/details")]
        HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("ListCollection");

        // --- GET endpoint ---
        if (req.Method == "GET")
        {
            var schedule = await _scheduleService.GetList();

            if (schedule == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(schedule);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
