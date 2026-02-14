using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Asset;

public class AssetAggCollectionFunction
{
    private readonly IAssetService _assetService;

    public AssetAggCollectionFunction(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [Function("AssetAggCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "asset/aggregate")] HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("AssetAggCollection");

        // GET /asset/aggregate
        if (req.Method == "GET")
        {
            var asset = await _assetService.GetAllAgg();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(asset);
            return ok;
        }

        // POST /asset/aggregate
        if (req.Method == "POST")
        {
            var (data, errorResponse) =
                await req.ReadJsonBodyAsync<Domain.DTOs.AssetAggregate>();

            if (errorResponse != null)
                return errorResponse;

            var created = await _assetService.CreateAgg(data!);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(created);
            return response;
        }
        

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
