using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#nullable enable
namespace PokerMonteCarloAPI
{
    public class Monte : IMonte
    {
        public Response Carlo(Request request, int numberOfSimulations = 10_000)
        {
            var allCards = Utilities.GenerateAllCards().ToList();
            var remainingCards = RemovePlayerAndTableCards(allCards, request);
            
            
            
            // calculate each players best hand
            // first best hand but also 5 highest cards that make the hand
            // determine winner
            // keep score for how often each player has won
            // extra - keep track of each players hand over time, return what percentage of time we got certain hands
            
            for (var i = 0; i < numberOfSimulations; i++)
            {
                var shuffledRemainingCards = remainingCards.ToList().FisherYatesShuffle();
                var tableCards = Utilities.GenerateTableCards(request, shuffledRemainingCards);
                var players = Utilities.GeneratePlayers(tableCards, shuffledRemainingCards, request);
            }
            
            return new Response
            {
                Id = 1,
                Test = "hello world"
            };
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