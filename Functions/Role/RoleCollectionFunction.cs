using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Role;

public class RoleCollectionFunction
{
    private readonly IRoleService _roleService;

    public RoleCollectionFunction(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [Function("RoleCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "role")] HttpRequestData req,
        FunctionContext context)
    {
        var log = context.GetLogger("RoleCollection");

        // GET /role
        if (req.Method == "GET")
        {
            var role = await _roleService.GetAll();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(role);
            return ok;
        }

        // POST /role
        if (req.Method == "POST")
        {
            var (data, errorResponse) =
                await req.ReadJsonBodyAsync<Domain.Models.Role>();

            if (errorResponse != null)
                return errorResponse;

            var created = await _roleService.Create(data!);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(created);
            return response;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
