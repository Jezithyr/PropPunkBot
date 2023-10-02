using PropPunkShared.Core;

namespace PropPunkShared.Services;

public sealed class RandomService : ServiceBase
{
    private readonly Random _random = new();

    public int RollDice(int diceSides, int diceCount = 1)
    {
        var roll = 0;
        for (int i = 0; i < diceCount; i++)
        {
            roll += _random.Next(diceSides+1);
        }
        return roll;
    }

}
