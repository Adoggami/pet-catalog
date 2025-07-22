using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;

namespace PetCatalog.Functions.Extensions;

public static class HttpResponseExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static async Task<HttpResponseData> CreateJsonResponseAsync<T>(
        this HttpRequestData req, 
        T data, 
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var response = req.CreateResponse(statusCode);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        
        var json = JsonSerializer.Serialize(data, JsonOptions);
        await response.WriteStringAsync(json);
        
        return response;
    }

    public static async Task<HttpResponseData> CreateErrorResponseAsync(
        this HttpRequestData req, 
        string error, 
        HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return await req.CreateJsonResponseAsync(new { error }, statusCode);
    }

    public static async Task<HttpResponseData> CreateNotFoundResponseAsync(
        this HttpRequestData req, 
        string message)
    {
        return await req.CreateErrorResponseAsync(message, HttpStatusCode.NotFound);
    }

    public static async Task<HttpResponseData> CreateCreatedResponseAsync<T>(
        this HttpRequestData req, 
        T data)
    {
        return await req.CreateJsonResponseAsync(data, HttpStatusCode.Created);
    }

    public static async Task<T?> ReadJsonBodyAsync<T>(this HttpRequestData req) where T : class
    {
        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            
            if (string.IsNullOrWhiteSpace(requestBody))
                return null;

            return JsonSerializer.Deserialize<T>(requestBody, JsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
