using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerMonteCarloAPI
{
    public class Monte : IMonte
    {
        public List<PlayerResult> Carlo(Request request)
        {
            const int numberOfSimulations = 100_000;
            var allCards = Utilities.GenerateAllCards().ToList();
            var remainingCards = RemovePlayerAndTableCards(allCards, request);
            
            // calculate each players best hand
            // first best hand but also 5 highest cards that make the hand
            // determine winner
            // keep score for how often each player has won
            // extra - keep track of each players hand over time, return what percentage of time we got certain hands
            
            var winTracker = new List<PlayerResult>();
            for (var i = 0; i < request.Players.Count; i++)
            {
                winTracker.Add(new PlayerResult());
            }
            
            for (var i = 0; i < numberOfSimulations; i++)
            {
                var shuffledRemainingCards = remainingCards.ToList().FisherYatesShuffle();
                var tableCards = Utilities.GenerateTableCards(request, shuffledRemainingCards);
                var players = Utilities.GeneratePlayers(tableCards, shuffledRemainingCards, request);
                var playersBestHands = players.Select(player => player.CalculateBestHand()).ToList();
                
                if(playersBestHands[0].Item1 > playersBestHands[1].Item1)
                    winTracker[0].winCount++;
                else if (playersBestHands[0].Item1 == playersBestHands[1].Item1)
                {
                    for(var j = 0; j < playersBestHands[0].Item2.Count; j++)
                    {
                        if (playersBestHands[0].Item2[j] > playersBestHands[1].Item2[j])
                        {
                            winTracker[0].winCount++;
                            break;
                        }
                    }
                }
            }
            
            return winTracker;
        }

        private List<Card> RemovePlayerAndTableCards(IEnumerable<Card> allCards, Request request)
        {
            var setOfAllCards = new HashSet<Card>(allCards);

            foreach (var card in request.TableCards)
            {
                setOfAllCards.Remove(card);
            }
            
            foreach (var card in request.Players.SelectMany(player => player.Cards))
            {
                setOfAllCards.Remove(card);
            }

            return setOfAllCards.ToList();
        }
    }
}