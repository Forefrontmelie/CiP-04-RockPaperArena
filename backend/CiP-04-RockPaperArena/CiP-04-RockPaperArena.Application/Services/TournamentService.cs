using CiP_04_RockPaperArena.Domain.Dtos;
using CiP_04_RockPaperArena.Domain.Interfaces;
using CiP_04_RockPaperArena.Domain.Models;
using System.ComponentModel;

namespace CiP_04_RockPaperArena.Application.Services;


public class TournamentService : ITournamentService
{
    public ITournamentRepository TournamentRepository { get; }
    public IParticipantRepository ParticipantRepository { get; }
    public IPairingStrategy PairingStrategy { get; }
    public IGameService GameService { get; }

    private IList<Participant> Participants { get; set; }


    public bool HasActiveTournament => TournamentRepository.HasActiveTournament();

    public Tournament? GetCurrentTournament() => TournamentRepository.GetCurrentTournament();

    public TournamentService(IParticipantRepository participantRepository, ITournamentRepository tournamentRepository, IPairingStrategy pairingStrategy, IGameService gameService)
    {
        ParticipantRepository = participantRepository;
        TournamentRepository = tournamentRepository;
        PairingStrategy = pairingStrategy;
        GameService = gameService;
        Participants = new List<Participant>();

        UpdateTSParticipantsList();
    }



    public void UpdateTSParticipantsList()
    {
        var currentTournament = TournamentRepository.GetCurrentTournament();

        if (currentTournament != null && currentTournament.IsActive)
        {
            Participants = currentTournament.Participants;
            Console.WriteLine("Participants updated from current tournament.");
        }
        else 
        {
        Participants = new List<Participant>(ParticipantRepository.GetAllParticipants());
        }
    }


    public void StartTournament(string name, int players)
    {
        var currentTournament = TournamentRepository.GetCurrentTournament();

        // Validation
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Player name cannot be empty", nameof(name));
        
        if (players <= 0 || players % 2 != 0)
            throw new ArgumentException("Number of players must be a positive even number", nameof(players));

        if (currentTournament?.IsActive == true)
            throw new InvalidOperationException("A tournament is already in progress");

        // Check now when using seeded players
        if (players > ParticipantRepository.GetAllParticipants().Count)
            throw new ArgumentException("Number of players exceeds the number of seeded players. Please enter smaller number.", nameof(players));


        // Get all available participants from repository
        var availableParticipants = ParticipantRepository.GetAllParticipants().ToList();
        
        if (availableParticipants.Count < players - 1)
            throw new InvalidOperationException($"Not enough participants available. Need {players - 1} AI participants, but only {availableParticipants.Count} available");

        // Create tournament participants list
        var tournamentParticipants = new List<Participant>();
        
        // Add the human player (get next available ID)
        int humanPlayerId = availableParticipants.Any() ? availableParticipants.Max(p => p.Id) + 1 : 1;
        tournamentParticipants.Add(new HumanParticipant(name, humanPlayerId));         //  <<<<<------ LÄGGS ENBART TILL I TS-PARTICIPANTS - LÄGGA TILL I PARTICIPANTREPOSITORY OCKSÅ?
        
        // Add AI participants (take the first n-1 from repository)
        var selectedAI = availableParticipants.Take(players - 1).ToList();
        tournamentParticipants.AddRange(selectedAI);
        
        // Create and start the tournament
        var tournament = new Tournament(name, humanPlayerId, tournamentParticipants);        
        TournamentRepository.SaveTournament(tournament);

        UpdateTSParticipantsList();    // Uppdaterar Participants-listan här i TournamentService 
        GenerateAllPlayerScheduleMap(); // Genererar schema för alla rundor direkt vid start        
    }

    public void GenerateAllPlayerScheduleMap()
    {
        var currentTournament = TournamentRepository.GetCurrentTournament();
        if (currentTournament == null || !currentTournament.IsActive)
            throw new InvalidOperationException("No active tournament");


        var scheduleMap = new Dictionary<int, IList<Match>>();
        int totalRounds = GetMaxNumberOfRounds(Participants.Count);

        for (int round = 1; round <= totalRounds; round++)
        {
            var matches = GetMatchListForSpecificRound(round);
            scheduleMap.Add(round, matches);
        }

        currentTournament.RoundSchedule = scheduleMap;
        TournamentRepository.SaveTournament(currentTournament);
    }





    public void AdvanceRound()
    {
        var currentTournament = TournamentRepository.GetCurrentTournament();
        if (currentTournament == null || !currentTournament.IsActive)
            throw new InvalidOperationException("No active tournament");

        var currentRound = currentTournament.CurrentRound;

        // Check: Ensure human match is complete before advancing
        if (!currentTournament.RoundSchedule.ContainsKey(currentRound))
            throw new InvalidOperationException($"Round {currentRound} schedule not found");

        var humanMatch = currentTournament.RoundSchedule[currentRound][0];
        if (!humanMatch.IsComplete)
            throw new InvalidOperationException("Cannot advance: Human match is not complete yet");

        // Check: Ensure AI matches haven't already been played for this round
        var aiMatches = currentTournament.RoundSchedule[currentRound].Skip(1).ToList();
        bool allAiMatchesComplete = aiMatches.All(m => m.IsComplete);

        if (!allAiMatchesComplete)
        {
            // Play AI matches only if they haven't been completed yet
            Console.WriteLine($"Playing AI matches for round {currentRound}");
            GameService.PlayAiMatchesCurrentRound(currentTournament.RoundSchedule, currentRound);
        }
        else
        {
            throw new InvalidOperationException("Cannot advance: AI matches for round {currentRound} already complete"); 
            Console.WriteLine($"AI matches for round {currentRound} already complete, skipping");
        }

        // Update scoreboard only once per round
        // Check if this round has already been scored
        if (!currentTournament.ScoredRounds.Contains(currentRound))
        {
            Console.WriteLine($"Updating scoreboard for round {currentRound}");
            currentTournament.Scoreboard = GameService.UpdateScoreboard(
                currentTournament.RoundSchedule,
                currentTournament.Scoreboard,
                currentRound
            );
            currentTournament.ScoredRounds.Add(currentRound);
        }
        else
        {
            Console.WriteLine($"Round {currentRound} already scored, skipping scoreboard update");
        }

        // Advance to next round
        if (currentRound < currentTournament.TotalRounds)
        {
            currentTournament.CurrentRound++;        /// !!!!!!!   INTE SÄKER PÅ ATT DENNA SKA VARA HÄR !!!!
            Console.WriteLine($"Advanced to round {currentTournament.CurrentRound} --- OBS! currentRound++ bortplockad här pga stoppar uppdateringen mid-match för human annars");
        }
        else
        {
            currentTournament.IsActive = false;
            currentTournament.IsFinished = true;
            Console.WriteLine("Tournament finished");
        }

        TournamentRepository.SaveTournament(currentTournament);
    }



    public void PerformAiMatches()
    {
        var currentTournament = TournamentRepository.GetCurrentTournament();
        var currentRound = currentTournament.CurrentRound;

        var updatedSchedule = GameService.PlayAiMatchesCurrentRound(currentTournament.RoundSchedule, currentRound);

        currentTournament.RoundSchedule = updatedSchedule;
        UpdateScoreboard();
        TournamentRepository.SaveTournament(currentTournament);
    }





    public void UpdateScoreboard()
    {
        var currentTournament = TournamentRepository.GetCurrentTournament();
        if (currentTournament == null || !currentTournament.IsActive)
            throw new InvalidOperationException("No active tournament");

        var currentRound = currentTournament.CurrentRound;

        var currentScoreboard = currentTournament.Scoreboard;

        currentTournament.Scoreboard = GameService.UpdateScoreboard(currentTournament.RoundSchedule, currentScoreboard, currentRound);
        TournamentRepository.SaveTournament(currentTournament);

    }


    public ScoreboardDTO GetScoreboard()
    {
        var currentTournament = TournamentRepository.GetCurrentTournament();
        if (currentTournament == null || !currentTournament.IsActive)
            throw new InvalidOperationException("No active tournament");

        var currentRound = currentTournament.CurrentRound;

        var currentScoreboard = currentTournament.Scoreboard;

        return new ScoreboardDTO(currentScoreboard); 

    }








    public int GetCurrentRoundNumber()
    {
        var currentTournament = TournamentRepository.GetCurrentTournament();
        return currentTournament?.CurrentRound ?? 0;
    }


    public Match PlayMove(int intMove)   // LÄGG TILL best of 3   --   och ÄNDRA TILL MatchResultDTO!
    {
        var currentTournament = TournamentRepository.GetCurrentTournament();
        if (currentTournament == null || !currentTournament.IsActive)
            throw new InvalidOperationException("No active tournament");

        Move move = (Move)intMove;  // Convert int to Move enum
        var currentRound = currentTournament.CurrentRound;

        var matchesThisRound = currentTournament.RoundSchedule[currentRound];
        var humanMatch = matchesThisRound[0];

        if (humanMatch.IsComplete)
            throw new InvalidOperationException("Your match is already complete.");


        var updatedSchedule = GameService.PlayHumanMatchCurrentRound(currentTournament.RoundSchedule, currentRound, move);

        currentTournament.RoundSchedule = updatedSchedule;
        TournamentRepository.SaveTournament(currentTournament);

        var updatedHumanMatch = updatedSchedule[currentRound][0];

        return updatedHumanMatch;
    }


    public StatusDTO GetHumanPlayersCurrentGameStatus()
    {
        var currentTournament = TournamentRepository.GetCurrentTournament();
        if (currentTournament == null || !currentTournament.IsActive)
            throw new InvalidOperationException("No active tournament");

        var currentRound = currentTournament.CurrentRound;

        var hm = currentTournament.RoundSchedule[currentRound][0];

        return new StatusDTO(hm.Player, hm.Opponent, hm.currentRound, hm.subRound, hm.player1Wins, hm.player2Wins, hm.draw, hm.IsComplete);

    }













    public int GetMaxNumberOfRounds(int n)
    {
        //int n = participants.Count;
        if (n == 0 || n % 2 != 0) return 0;
        return n - 1;
    }


    public int GetOpponentIndex(int id, int roundNbr)
    {
        if (roundNbr <= 0)
            throw new ArgumentOutOfRangeException(nameof(roundNbr));
        if (id < 0 || id > Participants.Max(p => p.Id))                     // TODO: <------------------------     FUNKAR?
            throw new ArgumentOutOfRangeException(nameof(id));

        int index = ConvertIdToIndex(id);

        return PairingStrategy.GetOpponentIndex(index, Participants.Count, roundNbr);
    }



    public Participant GetOpponentParticipant(int id, int roundNumber)
    {
        int index = ConvertIdToIndex(id);
        int opponentIndex = PairingStrategy.GetOpponentIndex(index, Participants.Count, roundNumber);
        return GetParticipant(opponentIndex);
    }

    public Participant GetParticipant(int index)
    {
        UpdateTSParticipantsList();

        if (index < 0 || index >= Participants.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        return Participants[index];
    }

    public Participant GetParticipantById(int id)
    {
        UpdateTSParticipantsList();

        if (id < 0 || id > Participants.Max(p => p.Id))
            throw new ArgumentOutOfRangeException(nameof(id));

        int index = ConvertIdToIndex(id);

        //return Participants.FirstOrDefault(p => p.Id == id); 
        return Participants[index];
    }


    public IList<Participant> GetPairsForRound(int roundNbr)  
    {
        UpdateTSParticipantsList();

        if (roundNbr < 1 || roundNbr > GetMaxNumberOfRounds(Participants.Count))
        {
            throw new ArgumentException("Round number must be between 1 and " + GetMaxNumberOfRounds(ParticipantRepository.GetAllParticipants().Count));
        }

        return PairingStrategy.RotateParticipants(new List<Participant>(Participants), roundNbr);
    }


    public RoundDTO GetPairsForSpecificRound(int roundNbr)   
    {
        if (roundNbr <= 0)
            throw new ArgumentOutOfRangeException(nameof(roundNbr));

        IList<Participant> rotatedParticipants = GetPairsForRound(roundNbr);
        //IList<MatchPair> pairs = new List<MatchPair>();
        IList<PairDTO> pairs = new List<PairDTO>();

        for (int i = 0; i < rotatedParticipants.Count / 2; i++)
        {
            Participant Player1 = rotatedParticipants[i];
            Participant Player2 = rotatedParticipants[rotatedParticipants.Count - 1 - i];
            //pairs.Add(new MatchPair(roundNbr, Player1, Player2));
            pairs.Add(new PairDTO(Player1.Name, Player2.Name));
        }
        return new RoundDTO(roundNbr, pairs);
    }


    public IList<Match> GetMatchListForSpecificRound(int roundNbr)
    {
        if (roundNbr <= 0)
            throw new ArgumentOutOfRangeException(nameof(roundNbr));

        IList<Participant> rotatedParticipants = GetPairsForRound(roundNbr);
        IList<Match> pairs = new List<Match>();        

        for (int i = 0; i < rotatedParticipants.Count / 2; i++)
        {
            Participant Player1 = rotatedParticipants[i];
            Participant Player2 = rotatedParticipants[rotatedParticipants.Count - 1 - i];
            Match match = new Match(Player1, Player2);
            match.currentRound = roundNbr;
            pairs.Add(match);
        }
        return pairs;
    }








    public PlayerScheduleDTO GetPlayerScheduleDTO(int id)
    {
        int index = ConvertIdToIndex(id);

        Participant player = GetParticipant(index);
        string playerName = player.Name;

        int totalRounds = GetMaxNumberOfRounds(Participants.Count);
        IList<PlayerScheduleEntryDTO> schedule = new List<PlayerScheduleEntryDTO>();

        for (int round = 1; round <= totalRounds; round++)
        {
            int opponentIndex = PairingStrategy.GetOpponentIndex(index, Participants.Count, round);
            string opponent = Participants[opponentIndex].Name;

            //schedule.Add(new MatchPair(round, player, opponent));
            schedule.Add(new PlayerScheduleEntryDTO(round, opponent));
        }

        return new PlayerScheduleDTO(playerName, totalRounds, schedule);
    }





    public int GetRemainingUniquePairs(int roundsPlayed)
    {
        UpdateTSParticipantsList();
        int n = Participants.Count;
        if (n < 2) return 0;      // TODO:  <--------------------- Ändra till Exception !!!
        int totalPairs = n * (n - 1) / 2;
        int pairsPlayed = Math.Min(roundsPlayed, GetMaxNumberOfRounds(n)) * (n / 2);
        return Math.Max(0, totalPairs - pairsPlayed);
    }


    public int ConvertIdToIndex(int id)
    {
        UpdateTSParticipantsList();

        if (Participants == null || Participants.Count == 0)
            throw new InvalidOperationException("No participants available.");


        for (int i = 0; i < Participants.Count; i++)
        {
            Console.WriteLine("id: " + id);
            Console.WriteLine("Loop: " + i + ", participant id:" + Participants[i].Id);

            if (Participants[i].Id == id)
            {
                return i;
            }
        }
        throw new ArgumentOutOfRangeException(nameof(id), $"Participant with id {id} not found.");
    }





}

