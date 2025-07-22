using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using PetCatalog.Functions.Extensions;
using PetCatalog.Functions.Services.Interfaces;

namespace PetCatalog.Functions.Functions;

public class GetPets
{
    private readonly IPetService _petService;
    private readonly ILogger<GetPets> _logger;

    public GetPets(IPetService petService, ILogger<GetPets> logger)
    {
        _petService = petService;
        _logger = logger;
    }

    [Function("GetPets")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "pets")] HttpRequestData req,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("GetPets function processed a request.");

        try
        {
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var limit = int.TryParse(query["limit"], out var l) ? l : 100;
            var offset = int.TryParse(query["offset"], out var o) ? o : 0;

            var result = await _petService.GetPetsAsync(limit, offset, cancellationToken);

            return result.Success 
                ? await req.CreateJsonResponseAsync(result.Data)
                : await req.CreateErrorResponseAsync(result.Error ?? "Unknown error");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetPets function");
            return await req.CreateErrorResponseAsync("An error occurred while processing the request", System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
