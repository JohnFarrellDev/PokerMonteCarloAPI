using System.Collections.Generic;

namespace PokerMonteCarloAPI
{
    public static class Constants
    {
        public static readonly IDictionary<GameStage, int> MapGameStageToExpectedTableCards =
            new Dictionary<GameStage, int>
            {
                { GameStage.PreFlop, 0 },
                { GameStage.Flop, 3 },
                { GameStage.Turn, 4 },
                { GameStage.River, 5 }
            };

        public static readonly IDictionary<GameStage, string> MapGameStageToDisplayValue = new Dictionary<GameStage, string>
        {
            { GameStage.PreFlop, "Preflop" },
            { GameStage.Flop, "Flop" },
            { GameStage.Turn, "Turn" },
            { GameStage.River, "River" }
        };
    }
}