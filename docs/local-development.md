# Avvio Ambiente di Sviluppo Locale

## 🚀 Avvio Rapido

### Opzione 1: Task VS Code (Raccomandato)
1. Premi `Cmd+Shift+P` (Mac) o `Ctrl+Shift+P` (Windows/Linux)
2. Digita "Tasks: Run Task"
3. Seleziona "start local environment"

Questo avvierà automaticamente:
- PostgreSQL in Docker
- Azure Functions in locale

### Opzione 2: Comandi Manuali

```bash
# 1. Avvia PostgreSQL
docker-compose up -d postgres

# 2. Verifica che PostgreSQL sia avviato
docker-compose ps

# 3. Avvia le Azure Functions
cd src/PetCatalog.Functions
func start --verbose
```

### Opzione 3: Tutto insieme con Docker Compose

```bash
# Avvia tutti i servizi (PostgreSQL + pgAdmin)
docker-compose up -d

# Per vedere i logs
docker-compose logs -f

# Per fermare tutto
docker-compose down
```

## 🗄️ Accesso al Database

### PostgreSQL
- **Host**: localhost
- **Port**: 5432
- **Database**: petcatalog
- **Username**: postgres
- **Password**: postgres

### pgAdmin (Interfaccia Web)
- **URL**: http://localhost:8080
- **Email**: admin@petcatalog.local
- **Password**: admin

Per connettere pgAdmin a PostgreSQL:
1. Vai su http://localhost:8080
2. Login con le credenziali sopra
3. Aggiungi nuovo server:
   - **Name**: PetCatalog Local
   - **Host**: postgres (nome del container)
   - **Port**: 5432
   - **Database**: petcatalog
   - **Username**: postgres
   - **Password**: postgres

## 🧪 Test

```bash
# Esegui tutti i test
dotnet test

# Esegui solo i test unitari
dotnet test tests/PetCatalog.UnitTests

# Esegui solo i test di integrazione
dotnet test tests/PetCatalog.IntegrationTests
```

## 📡 API Endpoints

Quando le Azure Functions sono avviate, saranno disponibili su:
- **Base URL**: http://localhost:7071/api

### Endpoints disponibili:
- `GET /api/pets` - Lista tutti i pet
- `GET /api/pets/{id}` - Ottieni un pet specifico
- `POST /api/pets` - Crea un nuovo pet
- `PUT /api/pets/{id}` - Aggiorna un pet
- `DELETE /api/pets/{id}` - Elimina un pet

## 🛑 Fermare l'Ambiente

```bash
# Ferma solo il database
docker-compose stop postgres

# Ferma tutto
docker-compose down

# Ferma tutto e rimuovi i volumi (⚠️ elimina i dati)
docker-compose down -v
```

## 🔧 Troubleshooting

### PostgreSQL non si avvia
```bash
# Verifica se la porta 5432 è occupata
lsof -i :5432

# Rimuovi container esistenti
docker-compose down
docker-compose up -d postgres
```

### Functions non trovano il database
1. Verifica che PostgreSQL sia avviato: `docker-compose ps`
2. Verifica la connection string in `src/PetCatalog.Functions/local.settings.json`
3. Testa la connessione: `docker-compose exec postgres pg_isready -U postgres`

### Dati di test mancanti
Il database viene inizializzato automaticamente con dati di esempio. Se mancano:
```bash
# Ricrea il database
docker-compose down -v
docker-compose up -d postgres
```
