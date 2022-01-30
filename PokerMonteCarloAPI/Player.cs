using System.Collections.Generic;

namespace PokerMonteCarloAPI
{
    public class Player
    {
        public List<Card> playersHand = new List<Card>(7);

        public Player(Card firstCard, Card secondCard) //now will take two Card objects
        {
            playersHand.Add(firstCard);
            playersHand.Add(secondCard);
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