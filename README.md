# CiP-04-RockPaperArena

A Rock-Paper-Scissors tournament application with a React frontend and ASP.NET Core backend. Play against AI opponents in a round-robin tournament format.

## Project Structure

```
CiP-04-RockPaperArena/
├── backend/
│   └── CiP-04-RockPaperArena/
│       ├── CiP-04-RockPaperArena.Api/          # ASP.NET Core Web API
│       ├── CiP-04-RockPaperArena.Application/  # Business logic layer
│       ├── CiP-04-RockPaperArena.Domain/       # Domain models & interfaces
│       └── CiP-04-RockPaperArena.Infrastructure/ # Data access & repositories
└── frontend/
    └── rockpaperarena-app/                      # React frontend
```

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js](https://nodejs.org/) (v16 or higher)
- npm (comes with Node.js)

## Getting Started

### Backend Setup

1. Navigate to the backend directory:
   ```bash
   cd backend/CiP-04-RockPaperArena
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Run the API:
   ```bash
   dotnet run --project CiP-04-RockPaperArena.Api
   ```

The API will be available at:
- HTTPS: `https://localhost:7173`
- HTTP: `http://localhost:5173`
- Swagger UI: `https://localhost:7173/swagger`

### Frontend Setup

1. Navigate to the frontend directory:
   ```bash
   cd frontend/rockpaperarena-app
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Configure the API URL (optional):
   - The default API URL is `https://localhost:7173/Api`
   - To change it, edit [.env](frontend/rockpaperarena-app/.env):
     ```
     REACT_APP_API_BASE_URL=https://localhost:7173/Api
     ```

4. Start the development server:
   ```bash
   npm start
   ```

The app will open at `http://localhost:3000`

## How to Play

1. Click **Play Game** to start a new tournament
2. Enter your name
3. Choose the number of players (must be an even number)
4. Play Rock-Paper-Scissors against AI opponents
5. Each match is best-of-3 rounds
6. Complete all tournament rounds to see the final standings

## Game Rules

- **Win**: 3 points
- **Draw**: 1 point
- **Loss**: 0 points

## Architecture

The backend follows Clean Architecture principles with:
- **API Layer**: RESTful endpoints using ASP.NET Core
- **Application Layer**: Services implementing business logic ([TournamentService.cs](backend/CiP-04-RockPaperArena/CiP-04-RockPaperArena.Application/Services/TournamentService.cs), [GameService.cs](backend/CiP-04-RockPaperArena/CiP-04-RockPaperArena.Application/Services/GameService.cs))
- **Domain Layer**: Core models and interfaces
- **Infrastructure Layer**: Data persistence and algorithms ([RoundRobinPairingStrategy.cs](backend/CiP-04-RockPaperArena/CiP-04-RockPaperArena.Infrastructure/RoundRobinPairingStrategy.cs))

The frontend is built with:
- React 19
- Material-UI (MUI) components
- Custom HTTP client for API communication

## API Endpoints

- `POST /Api/tournament/start` - Start a new tournament
- `GET /Api/tournament/status` - Get current tournament status
- `POST /Api/tournament/play` - Submit a move
- `POST /Api/tournament/advance` - Advance to next round
- `GET /Api/tournament/final` - Get final results
- `GET /Api/participants` - List all participants
- `POST /Api/player` - Add a player
- `DELETE /Api/player/{id}` - Remove a player

## Development

### Running Tests

Backend:
```bash
cd backend/CiP-04-RockPaperArena
dotnet test
```

Frontend:
```bash
cd frontend/rockpaperarena-app
npm test
```

### Building for Production

Backend:
```bash
cd backend/CiP-04-RockPaperArena
dotnet publish -c Release
```

Frontend:
```bash
cd frontend/rockpaperarena-app
npm run build
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.