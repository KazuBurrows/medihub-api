using System.Net;
using System.Text.Json;
using MediHub.Application.Interfaces;
using MediHub.Domain.Models;
using MediHub.Functions.Helpers;
using MediHub.Functions.Helpers.Exceptions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

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

        // For POST and PUT, read body instead of query string
        var body = await req.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body))
            return await HttpResponses.BadRequest(req, "Missing request body");

        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        if (!root.TryGetProperty("sessionId", out var sessionIdEl) || !sessionIdEl.TryGetInt32(out var sessionId))
            return await HttpResponses.BadRequest(req, "Invalid or missing sessionId");

        if (!root.TryGetProperty("assetId", out var assetIdEl) || !assetIdEl.TryGetInt32(out var assetId))
            return await HttpResponses.BadRequest(req, "Invalid or missing assetId");

        if (!root.TryGetProperty("startDatetime", out var startEl))
            return await HttpResponses.BadRequest(req, "Missing startDatetime");

        if (!root.TryGetProperty("endDatetime", out var endEl))
            return await HttpResponses.BadRequest(req, "Missing endDatetime");

        var startDatetime = startEl.GetString()!;
        var endDatetime = endEl.GetString()!;

        bool force = false;
        if (root.TryGetProperty("force", out var forceEl))
        {
            // Try as SByte (0/1)
            if (forceEl.ValueKind == JsonValueKind.Number && forceEl.TryGetSByte(out var sbyteVal))
            {
                force = sbyteVal != 0;
            }
            else if (forceEl.ValueKind == JsonValueKind.True)
            {
                force = true;
            }
            else if (forceEl.ValueKind == JsonValueKind.False)
            {
                force = false;
            }
            else
            {
                return await HttpResponses.BadRequest(req, "Invalid force value");
            }
        }
        else
        {
            return await HttpResponses.BadRequest(req, "Missing force value");
        }


        // Parse StaffDTO array from body
        List<StaffDTO> staffs = new List<StaffDTO>();

        if (root.TryGetProperty("staffs", out var staffsEl) && staffsEl.ValueKind == JsonValueKind.Array)
        {
            staffs = staffsEl.EnumerateArray()
                            .Select(s => new StaffDTO
                            {
                                Id = s.GetProperty("id").GetInt32(),
                                FirstName = s.TryGetProperty("firstName", out var f) ? f.GetString() : null,
                                LastName = s.TryGetProperty("lastName", out var l) ? l.GetString() : null,
                                Code = s.TryGetProperty("code", out var c) ? c.GetString() : null,
                                Email = s.TryGetProperty("email", out var e) ? e.GetString() : null,
                                RoleId = s.TryGetProperty("roleId", out var r) ? r.GetInt32() : null,
                                RoleName = s.TryGetProperty("roleName", out var rn) ? rn.GetString() : null
                            })
                            .ToList(); // <-- convert to List<StaffDTO>
        }


        if (req.Method == "PUT")
{
    log.LogInformation("PUT request received for InstanceDetailItem");
    log.LogInformation($"id: {id}");
    log.LogInformation($"sessionId: {sessionId}");
    log.LogInformation($"assetId: {assetId}");
    log.LogInformation($"startDatetime: {startDatetime}");
    log.LogInformation($"endDatetime: {endDatetime}");
    log.LogInformation($"force: {force}");

    if (staffs != null && staffs.Any())
    {
        log.LogInformation("Staffs:");
        foreach (var s in staffs)
        {
            log.LogInformation($"  Id: {s.Id}, FirstName: {s.FirstName}, LastName: {s.LastName}, Code: {s.Code}, RoleId: {s.RoleId}, RoleName: {s.RoleName}");
        }
    }
    else
    {
        log.LogInformation("Staffs: empty or null");
    }

    var instance = await _scheduleService.PutInstanceDetailDTO(id, sessionId, assetId, startDatetime, endDatetime, staffs, force);

    if (instance == null)
    {
        log.LogWarning("PutInstanceDetailDTO returned null (Not Found)");
        return req.CreateResponse(HttpStatusCode.NotFound);
    }

    log.LogInformation("PutInstanceDetailDTO returned instance:");
    log.LogInformation(JsonSerializer.Serialize(instance));

    var ok = req.CreateResponse(HttpStatusCode.OK);
    await ok.WriteAsJsonAsync(instance);
    return ok;
}


        if (req.Method == "POST")
        {
            try
            {
                var instance = await _scheduleService.CreateInstanceDetailDTO(sessionId, assetId, startDatetime, endDatetime, staffs, force);

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
