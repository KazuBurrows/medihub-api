using System.Net;
using Microsoft.Azure.Functions.Worker.Http;

public static class ApiResponseFactory
{
    private static async Task<HttpResponseData> Create(
        HttpRequestData req,
        HttpStatusCode status,
        string title,
        string detail,
        Dictionary<string, object>? extensions = null)
    {
        var response = req.CreateResponse(status);

        var body = new ApiResponse
        {
            Title = title,
            Status = (int)status,
            Detail = detail,
            Extensions = extensions
        };

        await response.WriteAsJsonAsync(body);
        return response;
    }

    public static Task<HttpResponseData> Ok(
        HttpRequestData req,
        string message)
        => Create(req, HttpStatusCode.OK, "Success", message);


    public static Task<HttpResponseData> NotFound(
        HttpRequestData req,
        string message)
        => Create(req, HttpStatusCode.NotFound, "Not Found", message);

    public static Task<HttpResponseData> BadRequest(
        HttpRequestData req,
        string message)
        => Create(req, HttpStatusCode.BadRequest, "Bad Request", message);


    public static Task<HttpResponseData> Success(
        HttpRequestData req,
        string entity,
        int id,
        ActionType action)
    {
        string actionStr = action.ToString().ToLower();

        return Create(
            req,
            HttpStatusCode.OK,
            $"Successfully {char.ToUpper(actionStr[0]) + actionStr[1..]}",
            $"{entity} with id {id} was successfully {actionStr}.",
            new Dictionary<string, object> { { "id", id } }
        );
    }

    public static Task<HttpResponseData> Success<T>(
        HttpRequestData req,
        string entity,
        T item,
        ActionType action)
    {
        string actionStr = action.ToString().ToLower();

        return Create(
            req,
            HttpStatusCode.OK,
            $"Successfully {char.ToUpper(actionStr[0]) + actionStr[1..]}",
            $"{entity} was successfully {actionStr}.",
            new Dictionary<string, object>
            {
                { "item", item } // Send the whole item back
            }
        );
    }


    public static Task<HttpResponseData> Conflict(
        HttpRequestData req,
        string entity,
        List<int> ids)
    {
        var extensions = new Dictionary<string, object>
        {
            { "ids", ids }
        };

        return Create(
            req,
            HttpStatusCode.Conflict,
            "Conflict",
            $"{entity} with ids {string.Join(", ", ids)} had a conflict.",
            extensions
        );
    }
}