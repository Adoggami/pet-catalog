# Pet Catalog C# Microservice

A serverless microservice for managing a pet catalog using Azure Functions (.NET 9) and PostgreSQL.

## Architecture

This microservice follows Clean Architecture principles with the following layers:

### 📁 Project Structure

```
pet-service-cs/
├─ src/
│  ├─ PetCatalog.Functions/          ← Azure Functions project
│  │  ├─ Functions/                  ← HTTP trigger functions
│  │  │  ├─ GetPets.cs
│  │  │  ├─ GetPet.cs
│  │  │  ├─ CreatePet.cs
│  │  │  ├─ UpdatePet.cs
│  │  │  └─ DeletePet.cs
│  │  ├─ Models/                     ← DTOs and API models
│  │  │  ├─ PetDto.cs
│  │  │  ├─ CreatePetRequest.cs
│  │  │  └─ UpdatePetRequest.cs
│  │  ├─ Services/                   ← Application services + DI
│  │  │  ├─ Interfaces/
│  │  │  │  └─ IPetService.cs
│  │  │  └─ PetService.cs
│  │  ├─ Extensions/                 ← Extension methods, middleware
│  │  │  └─ ServiceCollectionExtensions.cs
│  │  ├─ Program.cs                  ← Entry point + HostBuilder
│  │  ├─ PetCatalog.Functions.csproj ← Project file
│  │  ├─ host.json                   ← Functions host configuration
│  │  └─ local.settings.json         ← Local settings/secrets
│  ├─ PetCatalog.Domain/             ← Pure domain logic
│  ├─ PetCatalog.Application/        ← Use cases, orchestration
│  └─ PetCatalog.Infrastructure/     ← Data access, external services
│
├─ tests/
│  ├─ PetCatalog.UnitTests/
│  └─ PetCatalog.IntegrationTests/
│
├─ docs/                             ← Architecture docs
│  └─ architecture.md
│
├─ global.json                       ← .NET SDK pin
├─ PetCatalog.sln
└─ README.md
```

## Features

- RESTful API for CRUD operations on pets
- Serverless architecture using Azure Functions (.NET 9 isolated)
- PostgreSQL database with Entity Framework Core
- Clean Architecture with separation of concerns
- Comprehensive testing (unit + integration)
- Environment-based configuration

## API Endpoints

- `GET /api/pets` - Get all pets with pagination
- `GET /api/pets/{id}` - Get a pet by ID
- `POST /api/pets` - Create a new pet
- `PUT /api/pets/{id}` - Update a pet
- `DELETE /api/pets/{id}` - Delete a pet

## Development Setup

### Prerequisites

- .NET 9 SDK
- Azure Functions Core Tools v4
- PostgreSQL (or Docker with provided docker-compose)
- Visual Studio 2022 or VS Code

### Local Development

1. **Clone and restore**:
   ```bash
   git clone <repository>
   cd pet-service-cs
   dotnet restore
   ```

2. **Start PostgreSQL** (using Docker):
   ```bash
   # Use the docker-compose from python-pet-catalog-api for consistency
   cd ../python-pet-catalog-api
   docker-compose up -d postgres
   ```

3. **Run the Functions**:
   ```bash
   cd src/PetCatalog.Functions
   func start
   ```

4. **Test the API**:
   ```bash
   # Get all pets
   curl http://localhost:7071/api/pets
   
   # Create a pet
   curl -X POST http://localhost:7071/api/pets \
     -H "Content-Type: application/json" \
     -d '{"name":"Buddy","species":"Dog","breed":"Golden Retriever","age":3}'
   ```

### Testing Deployed Function

After deployment to Azure, test the live API:

```bash
# Replace with your function app URL
FUNCTION_URL="https://func-petcatalog-prod.azurewebsites.net"
FUNCTION_KEY="your-function-key"

# Check if Function App is running
curl "$FUNCTION_URL/api/health?code=$FUNCTION_KEY"

# Get all pets
curl "$FUNCTION_URL/api/pets?code=$FUNCTION_KEY"

# Get specific pet
curl "$FUNCTION_URL/api/pets/1?code=$FUNCTION_KEY"

# Create new pet
curl -X POST "$FUNCTION_URL/api/pets?code=$FUNCTION_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Max",
    "species": "Dog", 
    "breed": "German Shepherd",
    "age": 5,
    "description": "Loyal and intelligent dog"
  }'
```

**Troubleshooting 404 errors:**

1. **Check Function App status** in Azure Portal
2. **Verify deployment** completed successfully:
   ```bash
   func azure functionapp list-functions func-petcatalog-prod
   ```
3. **Check Function App logs**:
   ```bash
   func azure functionapp logstream func-petcatalog-prod
   ```
4. **Verify .NET 9 runtime** is properly configured
5. **Check if all dependencies** are deployed correctly

**Get Function Key** from Azure Portal:
1. Go to Azure Portal → Function App → Functions → Any function → Function Keys
2. Copy the default key or create a new one

### Database Migrations

The application uses Entity Framework Core with code-first migrations:

```bash
# Navigate to the Functions project
cd src/PetCatalog.Functions

# Add migration (first time setup)
dotnet ef migrations add InitialCreate --project ../PetCatalog.Infrastructure --startup-project .

# Update database (local development)
dotnet ef database update --project ../PetCatalog.Infrastructure --startup-project .

# Generate SQL script (for production)
dotnet ef migrations script --project ../PetCatalog.Infrastructure --startup-project . --output ../../deploy/schema.sql
```

**For Azure deployment**, the database connection string should be set in the Function App settings:
- `POSTGRES_CONN`: Connection string to Azure PostgreSQL Flexible Server

**Initial database setup** on Azure:
```bash
# Set connection string for Azure PostgreSQL
$env:POSTGRES_CONN="Host=pg-adoggami-prod.postgres.database.azure.com;Database=postgres;Username=pgadmin;Password=your-password;SSL Mode=Require"

# Apply migrations to Azure database
dotnet ef database update --project src/PetCatalog.Infrastructure --startup-project src/PetCatalog.Functions
```

## Testing

```bash
# Run unit tests
dotnet test tests/PetCatalog.UnitTests

# Run integration tests
dotnet test tests/PetCatalog.IntegrationTests

# Run all tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Deployment

### Local Development with Azure Functions Core Tools

1. **Install Azure Functions Core Tools v4**:
   ```bash
   # Windows (via npm)
   npm install -g azure-functions-core-tools@4 --unsafe-perm true
   
   # Windows (via Chocolatey)
   choco install azure-functions-core-tools
   
   # macOS
   brew tap azure/functions
   brew install azure-functions-core-tools@4
   ```

2. **Build and run locally**:
   ```bash
   # From project root
   dotnet build
   
   # Start Functions runtime
   cd src/PetCatalog.Functions
   func start
   ```

3. **Or use VS Code tasks**:
   - Press `Ctrl+Shift+P` → `Tasks: Run Task` → `func: host start`

### Deploy to Azure using Azure Functions Core Tools

1. **Login to Azure**:
   ```bash
   az login
   func azure login
   ```

2. **Build the project** for .NET 9:
   ```bash
   cd src/PetCatalog.Functions
   dotnet publish --configuration Release --framework net9.0
   ```

3. **Deploy to the Function App** (created by Terraform):
   ```bash
   func azure functionapp publish func-petcatalog-prod --force
   ```

4. **Verify deployment**:
   ```bash
   # List deployed functions
   func azure functionapp list-functions func-petcatalog-prod
   
   # Check runtime version
   az functionapp config show --name func-petcatalog-prod --resource-group rg-adoggami-prod --query "netFrameworkVersion"
   ```

5. **Or use VS Code task**:
   - Press `Ctrl+Shift+P` → `Tasks: Run Task` → `deploy to azure`

### Automated Deployment with GitHub Actions

The repository includes a GitHub Actions workflow for automated deployment:

1. **Set up secrets** in your GitHub repository:
   - `AZURE_CLIENT_ID`: Service Principal client ID
   - `AZURE_CLIENT_SECRET`: Service Principal client secret  
   - `AZURE_SUBSCRIPTION_ID`: Azure subscription ID
   - `AZURE_TENANT_ID`: Azure tenant ID

2. **Automatic deployment**: Push to `main` branch triggers deployment

3. **Manual deployment**: Use `workflow_dispatch` trigger

### Infrastructure as Code

The Azure Function App `func-petcatalog-prod` is created by Terraform in the `/infra` folder:

```bash
cd ../infra
terraform init
terraform plan
terraform apply
```

**Note**: The infrastructure creates:
- Azure Function App (`func-petcatalog-prod`)
- PostgreSQL Flexible Server
- Storage Account for Function App
- Storage Account for media files

## Configuration

### Environment Variables

| Variable | Description | Required |
|----------|-------------|----------|
| `POSTGRES_CONN` | PostgreSQL connection string | Yes |
| `STORAGE_ACCOUNT_NAME` | Azure Storage account name | Yes |
| `STORAGE_ACCOUNT_KEY` | Azure Storage account key | Yes |
| `APPINSIGHTS_INSTRUMENTATIONKEY` | Application Insights key | No |

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=pgadmin;Username=pgadmin;Password=eF8_nOZr-SlMChU3ctc4"
  },
  "AzureStorage": {
    "AccountName": "devstoreaccount1",
    "AccountKey": "..."
  }
}
```

## Architecture Decisions

See [docs/architecture.md](docs/architecture.md) for detailed architectural decisions and patterns used.

## Contributing

1. Follow Clean Architecture principles
2. Write unit tests for new features
3. Update integration tests for API changes
4. Follow C# coding conventions
5. Update documentation for new endpoints

## Related Projects

- **Python Version**: `../python-pet-catalog-api` - Original Python implementation
- **Infrastructure**: `../infra` - Terraform infrastructure definitions
