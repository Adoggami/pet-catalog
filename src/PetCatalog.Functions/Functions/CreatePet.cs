using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using PetCatalog.Functions.Extensions;
using PetCatalog.Functions.Models;
using PetCatalog.Functions.Services.Interfaces;

namespace PetCatalog.Functions.Functions;

public class CreatePet
{
    private readonly IPetService _petService;
    private readonly ILogger<CreatePet> _logger;

    public CreatePet(IPetService petService, ILogger<CreatePet> logger)
    {
        _petService = petService;
        _logger = logger;
    }

    [Function("CreatePet")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "pets")] HttpRequestData req,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("CreatePet function processed a request.");

        try
        {
            var createRequest = await req.ReadJsonBodyAsync<CreatePetRequest>();
            
            if (createRequest == null)
                return await req.CreateErrorResponseAsync("Invalid or missing request body");

            var validationResult = _petService.ValidateCreateRequest(createRequest);
            if (validationResult.Errors.Any())
                return await req.CreateJsonResponseAsync(validationResult, System.Net.HttpStatusCode.BadRequest);

            var result = await _petService.CreatePetAsync(createRequest, cancellationToken);

            return result.Success 
                ? await req.CreateCreatedResponseAsync(result.Data)
                : await req.CreateErrorResponseAsync(result.Error ?? "Unknown error", System.Net.HttpStatusCode.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreatePet function");
            return await req.CreateErrorResponseAsync("An error occurred while processing the request", System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
