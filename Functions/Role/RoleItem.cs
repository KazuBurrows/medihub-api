using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Role;

public class RoleItem
{
    private readonly IRoleService _roleService;

    public RoleItem(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [Function("RoleItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "delete", "put", "options",
            Route = "role/{id}")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
        var log = context.GetLogger("RoleItem");
        
        // GET /role/{id}
        if (req.Method == "GET")
        {
            var role = await _roleService.GetById(id);

            if (role == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(role);
            return ok;
        }

        // DELETE /role/{id}
        if (req.Method == "DELETE")
        {
            try
            {
                await _roleService.Delete(id);
                return await ApiResponseFactory.Success(req, "Role", id, ActionType.Deleted);
            }
            catch (NotFoundException ex)
            {
                return await ApiResponseFactory.NotFound(req, ex.Message);
            }
        }

        // PUT /role/{id}
        if (req.Method == "PUT")
        {
            var (data, errorResponse) = await req.ReadJsonBodyAsync<Domain.Models.Role>();

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

            var updated = await _roleService.Update(data);

            if (updated == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            return await ApiResponseFactory.Success<Domain.Models.Role>(req, "Role", updated, ActionType.Updated);
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
