using CiP_04_RockPaperArena.Domain.Models;
using CiP_04_RockPaperArena.Domain.Interfaces;



namespace CiP_04_RockPaperArena.Infrastructure;

public class RoundRobinPairingStrategy : IPairingStrategy
{
 
    public IList<Participant> RotateParticipants(List<Participant> participants, int roundNbr)
    {
        if (participants == null || participants.Count < 2)
            return participants;

        if (roundNbr < 1)
            throw new ArgumentException("The number of the round must be >= 1");

        // Use shared rotation helper (returns a new rotated list)
        return RotateWithFixedFirst(participants, roundNbr);
    }


    // Position mirroring algorithm to find opponent index
    public int GetOpponentIndex(int index, int n, int roundNbr)
    {
        if (n < 2 || n % 2 != 0)
            throw new ArgumentException("n must be an even number >= 2");
        if (index < 0 || index >= n)
            throw new ArgumentException(nameof(index));
        if (roundNbr < 1 || roundNbr > n - 1)
            throw new ArgumentException(nameof(roundNbr));

        // Create index list and rotate it using the same algorithm as RotateParticipants
        List<int> indices = new List<int>(n);
        for (int i = 0; i < n; i++)
            indices.Add(i);

        var rotated = RotateWithFixedFirst(indices, roundNbr);

        // Find where the player sits in the rotated list, then mirror to get opponent
        int pos = rotated.IndexOf(index);
        if (pos < 0)
            throw new InvalidOperationException("Index not found after rotation (unexpected).");

        int opponentPos = n - 1 - pos;
        return rotated[opponentPos];
    }




    // Shared helper: rotate positions 1..n-1 left by (roundNbr-1) % (n-1), keep index 0 fixed.
    private static List<T> RotateWithFixedFirst<T>(List<T> items, int roundNbr)
    {
        if (items == null || items.Count < 2)
            return items;

        if (roundNbr < 1)
            throw new ArgumentException("roundNbr must be >= 1");

        int totalNbrParticipants = items.Count;
        int lastIndex = totalNbrParticipants - 1;
        int distance = (roundNbr - 1) % lastIndex; // left-rotation distance for indices 1..n-1

        // no rotation required
        if (distance == 0)
            return new List<T>(items);

        var result = new List<T>(totalNbrParticipants);
        result.Add(items[0]); // Put fixed participant back at front

        // newR[currentIndex] = oldR[(currentIndex + distance) % lastIndex] where oldR is items[1..n-1]
        for (int currentIndex = 0; currentIndex < lastIndex; currentIndex++)
        {
            int oldIndex = 1 + ((currentIndex + distance) % lastIndex);
            result.Add(items[oldIndex]);
        }

        return result;
    }
}

