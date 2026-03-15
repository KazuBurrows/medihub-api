using System.Globalization;
using System.Net;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Domain.DTOs;
using MediHub.Functions.Helpers;
using MediHub.Functions.Helpers.Exceptions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MediHub.Functions.Instance;

public class InstanceDTOItem
{
    private readonly IInstanceService _instanceService;

    public InstanceDTOItem(IInstanceService instanceService)
    {
        _instanceService = instanceService;
    }

    [Function("InstanceDTOItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "put", "delete","options",
                    Route = "instance/{id:int}/detail")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
        var log = context.GetLogger("InstanceDTOItem");

        // GET
        if (req.Method == "GET")
        {
            var instance = await _instanceService.GetByIdDTO(id);

            if (instance == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(instance);
            return ok;
        }

        // PUT
        if (req.Method == "PUT")
        {
            var (input, errorResponse) = await RequestValidator.ReadAndValidateAsync<Domain.DTOs.InstanceDTO>(req);
            if (errorResponse != null)
                return errorResponse;
            try
            {
                input.Id = id;
                var instance = await _instanceService.UpdateDTO(input);

                return await ApiResponseFactory.Success<Domain.DTOs.InstanceDTO>(req, "Instance", instance, ActionType.Updated);
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

        // DELETE
        if (req.Method == "DELETE")
        {
            try
            {
                await _instanceService.DeleteDTO(id);
                return await ApiResponseFactory.Success(req, "Instance", id, ActionType.Deleted);
            }
            catch (NotFoundException ex)
            {
                return await ApiResponseFactory.NotFound(req, ex.Message);
            }
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
