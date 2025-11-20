using CiP_04_RockPaperArena.Domain.Dtos;
using CiP_04_RockPaperArena.Domain.Interfaces;
using CiP_04_RockPaperArena.Domain.Models;

namespace CiP_04_RockPaperArena.Application.Services;


public class TournamentService : ITournamentService
{
    public IParticipantRepository ParticipantRepository { get; }
    public IPairingStrategy PairingStrategy { get; }
    public IGameService GameService { get; }

    private IList<Participant> Participants { get; set; }
    private Tournament? CurrentTournament { get; set; }


    public TournamentService(IParticipantRepository participantRepository, IPairingStrategy pairingStrategy, IGameService gameService)
    {
        ParticipantRepository = participantRepository;
        PairingStrategy = pairingStrategy;
        GameService = gameService;
        Participants = new List<Participant>();

        UpdateParticipantsList();
    }


    public void UpdateParticipantsList()
    {
        if (CurrentTournament != null && CurrentTournament.IsActive)
        {
            Participants = CurrentTournament.Participants;
            Console.WriteLine("Participants updated from current tournament.");
        }
        else 
        {
        Participants = new List<Participant>(ParticipantRepository.GetAllParticipants());
        }
    }


    public void StartTournament(string name, int players)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Player name cannot be empty", nameof(name));
        
        if (players <= 0 || players % 2 != 0)
            throw new ArgumentException("Number of players must be a positive even number", nameof(players));

        if (CurrentTournament?.IsActive == true)
            throw new InvalidOperationException("A tournament is already in progress");


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
        tournamentParticipants.Add(new HumanParticipant(name, humanPlayerId));
        
        // Add AI participants (take the first n-1 from repository)
        var selectedAI = availableParticipants.Take(players - 1).ToList();
        tournamentParticipants.AddRange(selectedAI);
        
        // Create and start the tournament
        CurrentTournament = new Tournament(name, tournamentParticipants);

        UpdateParticipantsList();
        
        // Generate the first round using the pairing strategy
        GenerateNextRound();
    }

    private void GenerateNextRound()
    {
        if (CurrentTournament == null || !CurrentTournament.IsActive)
            throw new InvalidOperationException("No active tournament");
            
        if (CurrentTournament.IsCompleted)
            throw new InvalidOperationException("Tournament is already completed");

        var rotatedParticipants = PairingStrategy.RotateParticipants(new List<Participant>(Participants), CurrentTournament.Rounds.Count + 1);
        Participants = rotatedParticipants;

        var pairs = GetPairsForSpecificRound(CurrentTournament.Rounds.Count + 1);
        
        CurrentTournament.Rounds.Add(pairs);
    }

    public bool HasActiveTournament => CurrentTournament?.IsActive == true;
    
    public Tournament? GetCurrentTournament() => CurrentTournament;
    
    public RoundDTO? GetCurrentRound()
    {
        if (CurrentTournament?.IsActive != true || CurrentTournament.Rounds.Count == 0)
            return null;
            
        return CurrentTournament.Rounds.LastOrDefault();
    }
    
    public List<PairDTO>? GetCurrentRoundPairs()
    {
        var currentRound = GetCurrentRound();
        return currentRound?.Pairs.ToList();
    }



    public void AdvanceTournament()
    {
        throw new NotImplementedException();
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
        UpdateParticipantsList();

        if (index < 0 || index >= Participants.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        return Participants[index];
    }

    public Participant GetParticipantById(int id)
    {
        UpdateParticipantsList();

        if (id < 0 || id > Participants.Max(p => p.Id))
            throw new ArgumentOutOfRangeException(nameof(id));

        int index = ConvertIdToIndex(id);

        //return Participants.FirstOrDefault(p => p.Id == id); 
        return Participants[index];
    }


    public IList<Participant> GetPairsForRound(int roundNbr)  
    {
        UpdateParticipantsList();

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


    public PlayerScheduleDTO GetPlayerSchedule(int id)
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
        UpdateParticipantsList();
        int n = Participants.Count;
        if (n < 2) return 0;      // TODO:  <--------------------- Ändra till Exception !!!
        int totalPairs = n * (n - 1) / 2;
        int pairsPlayed = Math.Min(roundsPlayed, GetMaxNumberOfRounds(n)) * (n / 2);
        return Math.Max(0, totalPairs - pairsPlayed);
    }


    public int ConvertIdToIndex(int id)
    {
        UpdateParticipantsList();

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



    // (Optional) Simple console printing methods similar to original controllers.
    public void PrintRound(int roundNumber)
    {
        var round = GetPairsForSpecificRound(roundNumber);
        Console.WriteLine($"The pairs for round {round.Round}:");
        foreach (var p in round.Pairs)
            //Console.WriteLine($"{p.Player1.Name} vs {p.Player2.Name}");
            Console.WriteLine($"{p.player1} vs {p.player2}");
    }

    public void PrintMessage(string message)
    {
        Console.WriteLine(message);
    }

}

