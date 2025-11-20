namespace CiP_04_RockPaperArena.Domain.Dtos;

public record ResultDTO(
    string Player1Name,
    string Player2Name,
    int Player1Wins,
    int Player2Wins,
    string? WinnerName,
    bool IsComplete,
    List<GameRoundDTO> GameRounds
);

public record GameRoundDTO(
    int GameNumber,
    string Player1Move,
    string Player2Move,
    string Result  // "Player1 wins", "Player2 wins", or "Draw"
);
