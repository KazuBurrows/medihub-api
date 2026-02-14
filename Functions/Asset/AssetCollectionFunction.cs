using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Asset;

public class AssetCollectionFunction
{
    private readonly IAssetService _assetService;

    public AssetCollectionFunction(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [Function("AssetCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "asset")] HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("AssetCollection");

        // GET /asset
        if (req.Method == "GET")
        {
            var asset = await _assetService.GetAll();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(asset);
            return ok;
        }

        // POST /asset
        if (req.Method == "POST")
        {
            var (data, errorResponse) =
                await req.ReadJsonBodyAsync<Domain.Models.Asset>();

            if (errorResponse != null)
                return errorResponse;

            var created = await _assetService.Create(data!);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(created);
            return response;
        }
        

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
