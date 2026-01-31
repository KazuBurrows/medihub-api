using System.Net;
using System.Security.Claims;
using MediHub.Functions.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker.Http;
using MediHub.Functions;

public class AuthAndCorsMiddleware : IFunctionsWorkerMiddleware
{
    private const string TenantId = "9b91b29c-7692-499e-a868-d3aecd589c5f";
    private const string Audience = "5075e85b-cc81-456a-86e4-470c95ab2b68";

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var request = await context.GetHttpRequestDataAsync();
        if (request == null)
        {
            await next(context);
            return;
        }

        // Handle CORS preflight
        if (request.Method == "OPTIONS")
        {
            var response = request.CreateResponse(HttpStatusCode.OK);
            AddCorsHeaders(response);
            context.GetInvocationResult().Value = response;
            return;
        }

        // Run Auth
        try
        {
            var (user, _) = await AuthHelper.CheckRequestAsync(
                request,
                context.GetLogger("Auth"),
                TenantId,
                Audience
            );

            // Attach user to context for all functions
            context.Items["User"] = user;
        }
        catch (AuthHelper.HttpResponseException ex)
        {
            AddCorsHeaders(ex.Response);
            context.GetInvocationResult().Value = ex.Response;
            return;
        }

        await next(context);

        // Add CORS headers to all responses
        if (context.GetInvocationResult().Value is HttpResponseData res)
        {
            AddCorsHeaders(res);
        }
    }

    private void AddCorsHeaders(HttpResponseData response)
    {
        response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:3000");
        response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,PUT,DELETE,OPTIONS");
        response.Headers.Add("Access-Control-Allow-Headers", "Content-Type,Authorization");
    }
}
