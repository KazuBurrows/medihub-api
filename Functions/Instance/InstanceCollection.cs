using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Instance;

public class InstanceCollection
{
    private readonly IInstanceService _instanceService;

    public InstanceCollection(IInstanceService instanceService)
    {
        _instanceService = instanceService;
    }

    [Function("InstanceCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "instances")] HttpRequestData req,
        FunctionContext context)
    { 
        var log = context.GetLogger("InstanceCollection");

        // GET /instance
        if (req.Method == "GET")
        {
            var instance = await _instanceService.GetAll();

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(instance);
            return ok;
        }

        // POST /instance
        if (req.Method == "POST")
        {
            var (input, errorResponse) = await RequestValidator.ReadAndValidateAsync<Domain.Models.Instance>(req);
            if (errorResponse != null)
                return errorResponse;

             try
            {
                var instance = await _instanceService.Create(input);
                return await ApiResponseFactory.Success<Domain.Models.Instance>(req, "Instance", instance, ActionType.Created);
            }
            catch (ConflictException ex)
            {
                return await ApiResponseFactory.Conflict(req, ex.Message, ex.ConflictingIds);
            }
            catch (NotFoundException ex)
            {
                return await ApiResponseFactory.NotFound(req, ex.Message);
            }
        }
        

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
