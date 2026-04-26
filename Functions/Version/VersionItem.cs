using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Version;

public class VersionItem
{
    private readonly IVersionService _versionService;

    public VersionItem(IVersionService versionService)
    {
        _versionService = versionService;
    }

    [Function("VersionItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "put", "options",
            Route = "version/{id}")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
         var log = context.GetLogger("VersionItem");

        // GET /version/{id}
        if (req.Method == "GET")
        {
            var version = await _versionService.GetById(id);

            if (version == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(version);
            return ok;
        }

        // DELETE /version/{id}
        if (req.Method == "DELETE")
        {
            try
            {
                await _versionService.Delete(id);
                return await ApiResponseFactory.Success(req, "Version", id, ActionType.Deleted);
            }
            catch (NotFoundException ex)
            {
                return await ApiResponseFactory.NotFound(req, ex.Message);
            }
        }

        // PUT /version/{id}
        if (req.Method == "PUT")
        {
            var (data, errorResponse) = await req.ReadJsonBodyAsync<Domain.Models.Version>();

            if (errorResponse != null)
                return errorResponse;

            // OPTIONAL: Validate body ID if it exists
            if (data!.Id != 0 && data.Id != id)
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync(
                    "ID in request body does not match route ID."
                );
                return bad;
            }

            // Force route ID to be authoritative
            data.Id = id;

            var updated = await _versionService.Update(data);

            if (updated == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            return await ApiResponseFactory.Success<Domain.Models.Version>(req, "Version", updated, ActionType.Updated);
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
