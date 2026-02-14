using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Asset;

public class AssetDTOCollectionFunction
{
    private readonly IAssetService _assetService;

    public AssetDTOCollectionFunction(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [Function("AssetDTOCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "asset/details")] HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("AssetDTOCollection");

        // GET /asset
        if (req.Method == "GET")
        {
            var asset = await _assetService.GetAllDTO();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(asset);
            return ok;
        }
        

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
