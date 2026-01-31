using System.Net;
using MediHub.Application.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Schedule;

public class ScheduleDTOCollectionFunction
{
    private readonly IScheduleService _scheduleService;

    public ScheduleDTOCollectionFunction(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [Function("ScheduleDTOCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "schedule/details/{year:int?}/{week:int?}")] HttpRequestData req,
            int? year,
            int? week,
            FunctionContext context)
    {
        var log = context.GetLogger("ScheduleDTOCollection");

        if (!year.HasValue)
        {
            year = DateTime.UtcNow.Year;
        }

        // GET /schedule
        if (req.Method == "GET")
        {
            var ok = req.CreateResponse(HttpStatusCode.OK);
            // return all schedules for the year
            var schedule = await _scheduleService.GetAllDTO(year.Value);
            await ok.WriteAsJsonAsync(schedule);
            return ok;
        }
        

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
