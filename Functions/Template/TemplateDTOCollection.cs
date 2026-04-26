using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Claims;
using MediHub.Application.Interfaces;
using MediHub.Common.Exceptions.Infrastructure;
using MediHub.Domain.DTOs;
using MediHub.Functions.Helpers;
using MediHub.Functions.Helpers.Exceptions;
using MediHub.Infrastructure.Data.Utils;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace MediHub.Functions.Template;

public class TemplateDTOCollection
{
    private readonly ITemplateService _templateService;

    public TemplateDTOCollection(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    [Function("TemplateDTOCollection")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get", "post", "options",
            Route = "templates/detail")] HttpRequestData req,
            FunctionContext context)
    {
        var log = context.GetLogger("TemplateDTOCollection");

        // GET /template
        if (req.Method == "GET")
        {
            var ok = req.CreateResponse(HttpStatusCode.OK);
            IEnumerable<TemplateDTO> templates;

            templates = await _templateService.GetAllDTO();

            await ok.WriteAsJsonAsync(templates);

            return ok;
        }

        if (req.Method == "POST")
        {
            var (input, errorResponse) = await RequestValidator.ReadAndValidateAsync<TemplateInputDTO>(req);
            if (errorResponse != null)
                return errorResponse;

            try
            {
                var template = await _templateService.CreateTemplateDTO(
                    input.SessionId,
                    input.AssetId,
                    input.CycleWeek,
                    input.CycleDay,
                    input.StartTime,
                    input.EndTime,
                    input.IsOpen,
                    input.Force,
                    input.VersionId
                );
                return await ApiResponseFactory.Success<TemplateDTO>(req, "Template", template, ActionType.Created);
            }
            catch (ConflictException ex)
            {
                return await ApiResponseFactory.Conflict(req, ex.Message, ex.ConflictingIds);
            }
        }
        

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
