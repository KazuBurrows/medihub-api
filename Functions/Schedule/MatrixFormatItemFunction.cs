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

public class MatrixFormatItemFunction
{
    private readonly IScheduleService _scheduleService;

    public MatrixFormatItemFunction(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [Function("MatrixFormatItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "schedule/matrix/format/{facilityId:int?}")]
        HttpRequestData req,
        int? facilityId,
        FunctionContext context)
    {
        var log = context.GetLogger("MatrixFormatItem");

        // --- Default values ---
        facilityId ??= 0;

        // --- GET endpoint ---
        if (req.Method == "GET")
        {
            var template = await _scheduleService.GetMatrixFormat((int)facilityId);

            if (template == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(template);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
