using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

#nullable enable
namespace PokerMonteCarloAPI
{
    public class Monte : IMonte
    {
        public Response Carlo(Request request, int numberOfSimulations)
        {
            var allCards = Utilities.GenerateAllCards().ToList();
            var remainingCards = RemovePlayerAndTableCards(allCards, request);
            
            // generate our players
            
            // calculate each players best hand
            // determine winner
            // keep score
            // extra - keep track of each players hand over time, return what percentage of time we got certain hands
            
            for (var i = 0; i < numberOfSimulations; i++)
            {
                var shuffledRemainingCards = remainingCards.ToList().FisherYatesShuffle();
                var tableCards = Utilities.GenerateTableCards(request, shuffledRemainingCards);
            }
            
            return new Response
            {
                Id = 1,
                Test = "hello world",
                Test2 = JsonSerializer.Serialize(request)
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