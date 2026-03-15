using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Functions.Worker.Http;

public static class RequestValidator
{
    public static async Task<(T? Value, HttpResponseData? ErrorResponse)> 
        ReadAndValidateAsync<T>(HttpRequestData req)
        where T : class
    {
        T? input;

        try
        {
            input = await req.ReadFromJsonAsync<T>();
        }
        catch
        {
            var badJson = await ProblemResponse.BadRequest(req, "Invalid JSON payload");
            return (null, badJson);
        }

        if (input == null)
        {
            var missingBody = await ProblemResponse.BadRequest(req, "Missing body");
            return (null, missingBody);
        }

        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(
            input,
            new ValidationContext(input),
            validationResults,
            validateAllProperties: true
        );

        if (!isValid)
        {
            var errorResponse = await ProblemResponse.BadRequest(
                req,
                string.Join("; ", validationResults.Select(v => v.ErrorMessage))
            );
            return (null, errorResponse);
        }

        return (input, null);
    }
}