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

        PerformAiMatches();
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


    public void PerformAiMatches()
    {
        var currentTournament = TournamentRepository.GetCurrentTournament();
        var currentRound = currentTournament.CurrentRound;

        var updatedSchedule = GameService.PlayAiMatchesCurrentRound(currentTournament.RoundSchedule, currentRound);

        currentTournament.RoundSchedule = updatedSchedule;
        TournamentRepository.SaveTournament(currentTournament);
    }


/*
    // Generates the next round of pairings and adds it to the current tournament
    private void GenerateNextRound()
    {
        var currentTournament = TournamentRepository.GetCurrentTournament();

        if (currentTournament == null || !currentTournament.IsActive)
            throw new InvalidOperationException("No active tournament");            
        if (currentTournament.IsCompleted)
            throw new InvalidOperationException("Tournament is already completed");

        var currentRound = currentTournament.CurrentRound;
        if (currentRound == null) return;


        var rotatedParticipants = PairingStrategy.RotateParticipants(new List<Participant>(Participants), currentRound + 1);
        Participants = rotatedParticipants;

        var pairs = GetMatchListForSpecificRound(currentRound + 1);

        currentTournament.RoundSchedule.Add(currentRound + 1, pairs);

        currentTournament.CurrentRound++;
        // Save the updated tournament
        TournamentRepository.SaveTournament(currentTournament);        
    }
*/

  



    // Returns the list of pairs for the current round
  /*  public List<PairDTO>? GetCurrentRoundPairs()
    {
        var currentTournament = TournamentRepository.GetCurrentTournament();
        var currentRound = currentTournament.CurrentRound;
        return currentRound?.Pairs.ToList();
    }
  */



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


    public StatusDTO GetPlayersCurrentGameStatus()
    {
        var currentTournament = TournamentRepository.GetCurrentTournament();
        if (currentTournament == null || !currentTournament.IsActive)
            throw new InvalidOperationException("No active tournament");

        var currentRound = currentTournament.CurrentRound;

        var hm = currentTournament.RoundSchedule[currentRound][0];

        return new StatusDTO(hm.Player, hm.Opponent, hm.currentRound, hm.player1Wins, hm.player1Wins, hm.draw, hm.IsComplete);

    }




    // Advances the tournament - simulates all the AI matches in the current round and generates the next round
    public void AdvanceTournament()
    {
        var currentTournament = TournamentRepository.GetCurrentTournament();

        if (currentTournament == null || !currentTournament.IsActive)
            throw new InvalidOperationException("No active tournament");

        var currentRound = currentTournament.CurrentRound;
        if (currentRound == null) return;



        // Simulate all AI vs AI matches using existing methods
        // <<<<-----------------------------------------------                              !!!!!!!! IMPLEMENT!


        // Generate next round if tournament not complete
        if (!currentTournament.IsCompleted)
        {
            currentTournament.CurrentRound++;
           // GenerateNextRound();
        }
        else
        {
            currentTournament.IsActive = false; // Tournament finished
        }

        TournamentRepository.SaveTournament(currentTournament);
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

