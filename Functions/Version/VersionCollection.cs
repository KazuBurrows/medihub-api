using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Version;

public class VersionCollection
{
    private readonly IVersionService _versionService;

    public VersionCollection(IVersionService versionService)
    {
        _versionService = versionService;
    }

    [Function("VersionCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "versions")] HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("VersionCollection");

        // GET /versions
        if (req.Method == "GET")
        {
            var version = await _versionService.GetAll();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(version);
            return ok;
        }

        // POST /versions
        if (req.Method == "POST")
        {
            var (data, errorResponse) =
                await req.ReadJsonBodyAsync<Domain.Models.Version>();

            if (errorResponse != null)
                return errorResponse;

            var created = await _versionService.Create(data!);

            return await ApiResponseFactory.Success<Domain.Models.Version>(req, "Version", created, ActionType.Created);
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
