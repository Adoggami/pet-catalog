using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using PetCatalog.Functions.Services.Interfaces;
using System.Net;
using System.Text.Json;

namespace PetCatalog.Functions.Functions;

public class DeletePet
{
    private readonly IPetService _petService;
    private readonly ILogger<DeletePet> _logger;

    public DeletePet(IPetService petService, ILogger<DeletePet> logger)
    {
        _petService = petService;
        _logger = logger;
    }

    [Function("DeletePet")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "pets/{id:int}")] HttpRequestData req,
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("DeletePet function processed a request for pet ID: {PetId}", id);

            var result = await _petService.DeletePetAsync(id, cancellationToken);

            if (!result.Success || !result.Data)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                notFoundResponse.Headers.Add("Content-Type", "application/json");
                
                var errorMessage = JsonSerializer.Serialize(new { error = $"Pet with ID {id} not found" });
                await notFoundResponse.WriteStringAsync(errorMessage);
                
                return notFoundResponse;
            }

            // Successo - ritorna 204 No Content
            var response = req.CreateResponse(HttpStatusCode.NoContent);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeletePet function for pet ID: {PetId}", id);
            
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            errorResponse.Headers.Add("Content-Type", "application/json");
            
            var errorMessage = JsonSerializer.Serialize(new { error = "An error occurred while processing the request" });
            await errorResponse.WriteStringAsync(errorMessage);
            
            return errorResponse;
        }
    }
}
