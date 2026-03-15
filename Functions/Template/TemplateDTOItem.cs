using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Domain.DTOs;
using MediHub.Functions.Helpers;
using MediHub.Functions.Helpers.Exceptions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace MediHub.Functions.Template;

public class TemplateDTOItem
{
    private readonly ITemplateService _templateService;

    public TemplateDTOItem(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    [Function("TemplateDTOItem")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "put", "options",
                    Route = "template/{id:int}/detail")] HttpRequestData req,
        int id,
        FunctionContext context)
    {
        var log = context.GetLogger("TemplateDTOItem");

        // GET
        if (req.Method == "GET")
        {
            var template = await _templateService.GetByIdDTO(id);

            if (template == null)
                return req.CreateResponse(HttpStatusCode.NotFound);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(template);
            return ok;
        }


        if (req.Method == "PUT")
        {
            var (input, errorResponse) = await RequestValidator.ReadAndValidateAsync<TemplateInputDTO>(req);
            if (errorResponse != null)
                return errorResponse;
            try
            {
                var template = await _templateService.PutTemplateDTO(
                    id,
                    input.SessionId,
                    input.AssetId,
                    input.CycleWeek,
                    input.CycleDay,
                    input.StartTime,
                    input.EndTime,
                    input.IsOpen,
                    input.Force
                );
                return await ApiResponseFactory.Success<TemplateDTO>(req, "Template", template, ActionType.Updated);
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
