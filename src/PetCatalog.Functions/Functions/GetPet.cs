using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using PetCatalog.Functions.Extensions;
using PetCatalog.Functions.Services.Interfaces;

namespace PetCatalog.Functions.Functions;

public class GetPet
{
    private readonly IPetService _petService;
    private readonly ILogger<GetPet> _logger;

    public GetPet(IPetService petService, ILogger<GetPet> logger)
    {
        _petService = petService;
        _logger = logger;
    }

    [Function("GetPet")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "pets/{id:int}")] HttpRequestData req,
        int id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GetPet function processed a request for pet ID: {PetId}", id);

        try
        {
            var result = await _petService.GetPetByIdAsync(id, cancellationToken);

            return result switch
            {
                { Success: true, Data: not null } => await req.CreateJsonResponseAsync(result.Data),
                { Success: true, Data: null } => await req.CreateNotFoundResponseAsync($"Pet with ID {id} not found"),
                _ => await req.CreateErrorResponseAsync(result.Error ?? "Unknown error")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetPet function for pet ID: {PetId}", id);
            return await req.CreateErrorResponseAsync("An error occurred while processing the request", System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
