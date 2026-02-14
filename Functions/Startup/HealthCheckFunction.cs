using System.Net;
using MediHub.Application.Interfaces;
using MediHub.Infrastructure.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Data.SqlClient;

namespace MediHub.Functions.Schedule;

public class HealthCheckFunction
{
    private readonly SqlConnectionFactory _connectionFactory;

    public HealthCheckFunction(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    [Function("HealthCheck")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "healthcheck")] HttpRequestData req)
    {
        var response = req.CreateResponse();

        try
        {
            await using var conn = await _connectionFactory.GetOpenConnectionAsync();

            response.StatusCode = HttpStatusCode.OK;
            await response.WriteStringAsync("SQL OK");
        }
        catch (Exception)
        {
            response.StatusCode = HttpStatusCode.ServiceUnavailable;
            await response.WriteStringAsync("SQL FAIL");
        }

        return response;
    }
}

