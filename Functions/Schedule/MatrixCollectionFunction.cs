using System.Globalization;
using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using MediHub.Infrastructure.Data.Utils;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MediHub.Functions.Schedule;

public class MatrixCollectionFunction
{
    private readonly IScheduleService _scheduleService;

    public MatrixCollectionFunction(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [Function("MatrixCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "schedule/matrix/{year:int?}/{week:int?}/{facility:int?}/{asset:int?}")]
        HttpRequestData req,
        int? year,
        int? week,
        int? facility,
        int? asset,
        FunctionContext context)
    {
        var log = context.GetLogger("MatrixCollection");

        // --- Default values ---
        year ??= DateTime.UtcNow.Year;

        week ??= WeekHelper.GetCurrentWeekOfYear((int)year);

        // facility/asset null means "all"
        int? facilityId = facility.HasValue && facility.Value > 0 ? facility.Value : null;
        int? assetId = asset.HasValue && asset.Value > 0 ? asset.Value : null;

        // --- GET endpoint ---
        if (req.Method == "GET")
        {
            var schedule = await _scheduleService.GetMatrix(year.Value, week.Value, facilityId, assetId);

            if (schedule == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(schedule);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
