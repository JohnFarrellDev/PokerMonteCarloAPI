using System.Collections.Generic;

#nullable enable
namespace PokerMonteCarloAPI
{
    public class Player
    {
        public readonly List<Card> PlayersHand;
        public bool Folded;
        
        public Player(List<Card> playersHand, bool folded)
        {
            PlayersHand = playersHand;
            Folded = folded;
        }
    }
}