using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Asset;

public class AssetDTOItem
{
    private readonly IAssetService _assetService;

    public AssetDTOItem(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [Function("AssetDTOItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "options",
            Route = "asset/{id}/detail")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
        var log = context.GetLogger("AssetDTOItem");


        if (req.Method == "GET")
        {
            var asset = await _assetService.GetByIdDTO(id);

            if (asset == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(asset);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
