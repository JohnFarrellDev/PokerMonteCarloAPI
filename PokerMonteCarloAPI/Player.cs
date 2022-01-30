using System.Collections.Generic;

#nullable enable
namespace PokerMonteCarloAPI
{
    public class Player
    {
        public List<Card> playersHand = null!;
        public bool Folded;

        public Player(List<Card> playersHand, bool folded)
        {
            this.playersHand = playersHand;
            Folded = folded;
        }
    }
}