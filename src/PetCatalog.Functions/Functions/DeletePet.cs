using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using PetCatalog.Functions.Extensions;
using PetCatalog.Functions.Services.Interfaces;

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
        _logger.LogInformation("DeletePet function processed a request for pet ID: {PetId}", id);

        try
        {
            var result = await _petService.DeletePetAsync(id, cancellationToken);
    
            return result switch
            {
                { Success: true, Data: true } => await req.CreateJsonResponseAsync(new { message = $"Pet with ID {id} deleted successfully" }, System.Net.HttpStatusCode.NoContent),
                { Success: true, Data: false } => await req.CreateNotFoundResponseAsync($"Pet with ID {id} not found"),
                _ => await req.CreateErrorResponseAsync(result.Error ?? "Unknown error", System.Net.HttpStatusCode.InternalServerError)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeletePet function for pet ID: {PetId}", id);
            return await req.CreateErrorResponseAsync("An error occurred while processing the request", System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
