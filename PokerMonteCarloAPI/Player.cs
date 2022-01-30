using System.Collections.Generic;

#nullable enable
namespace PokerMonteCarloAPI
{
    public class Player
    {
        public List<Card> playersHand = new List<Card>(7);
        public bool Folded;

        public Player(Card firstCard, Card secondCard, bool folded) //now will take two Card objects
        {
            playersHand.Add(firstCard);
            playersHand.Add(secondCard);
            Folded = folded;
        }

        public Player(List<Card> cardsPlayerHas) //create a player object from a list of cards
        {
            foreach(var card in cardsPlayerHas)
            {
                playersHand.Add(card);
            }
        }
    }
}