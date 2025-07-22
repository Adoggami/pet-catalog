using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using PetCatalog.Functions.Extensions;
using PetCatalog.Functions.Models;
using PetCatalog.Functions.Services.Interfaces;

namespace PetCatalog.Functions.Functions;

public class UpdatePet
{
    private readonly IPetService _petService;
    private readonly ILogger<UpdatePet> _logger;

    public UpdatePet(IPetService petService, ILogger<UpdatePet> logger)
    {
        _petService = petService;
        _logger = logger;
    }

    [Function("UpdatePet")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "pets/{id:int}")] HttpRequestData req,
        int id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("UpdatePet function processed a request for pet ID: {PetId}", id);

        try
        {
            var updateRequest = await req.ReadJsonBodyAsync<UpdatePetRequest>();
            
            if (updateRequest == null)
                return await req.CreateErrorResponseAsync("Invalid or missing request body");

            var validationResult = _petService.ValidateUpdateRequest(updateRequest);
            if (validationResult.Errors.Any())
                return await req.CreateJsonResponseAsync(validationResult, System.Net.HttpStatusCode.BadRequest);

            var result = await _petService.UpdatePetAsync(id, updateRequest, cancellationToken);

            return result switch
            {
                { Success: true, Data: not null } => await req.CreateJsonResponseAsync(result.Data),
                { Success: true, Data: null } => await req.CreateNotFoundResponseAsync($"Pet with ID {id} not found"),
                _ => await req.CreateErrorResponseAsync(result.Error ?? "Unknown error", System.Net.HttpStatusCode.InternalServerError)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdatePet function for pet ID: {PetId}", id);
            return await req.CreateErrorResponseAsync("An error occurred while processing the request", System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
