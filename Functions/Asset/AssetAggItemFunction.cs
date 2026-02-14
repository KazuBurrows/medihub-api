using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Asset;

public class AssetAggItemFunction
{
    private readonly IAssetService _assetService;

    public AssetAggItemFunction(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [Function("AssetAggItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "put", "options",
            Route = "asset/{id}/aggregate")] HttpRequestData req,
        string id,
        FunctionContext context)
    {
        var log = context.GetLogger("AssetAggItem");
        
        // Validate ID safely
        if (!int.TryParse(id, out var assetId))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid asset id.");
            return bad;
        }

        // GET /asset/{id}aggregate
        if (req.Method == "GET")
        {
            var asset = await _assetService.GetByIdAgg(assetId);

            if (asset == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(asset);
            return ok;
        }

        // DELETE /asset/{id}/aggregate
        if (req.Method == "DELETE")
        {
            var deleted = await _assetService.DeleteAgg(assetId);

            if (deleted == 0)
                return req.CreateResponse(HttpStatusCode.NotFound);

            return req.CreateResponse(HttpStatusCode.NoContent);
        }

        // PUT /asset/{id}/aggregate
        if (req.Method == "PUT")
        {
            var (data, errorResponse) = await req.ReadJsonBodyAsync<Domain.DTOs.AssetAggregate>();

            if (errorResponse != null)
                return errorResponse;

            // OPTIONAL: Validate body ID if it exists
            if (data!.Id != 0 && data.Id != assetId)
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync(
                    "ID in request body does not match route ID."
                );
                return bad;
            }

            // Force route ID to be authoritative
            data.Id = assetId;

            var updated = await _assetService.UpdateAgg(data);

            if (updated == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(updated);
            return ok;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
