namespace CiP_04_RockPaperArena.Domain.Dtos;

public record StatusDTO(string player, string opponent, int currentRound, int player1Wins, int player2Wins, int draws, bool isComplete);