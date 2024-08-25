using System;
using System.Collections.Generic;
using System.Linq;
using PokerMonteCarloAPI.Services;

#nullable enable
namespace PokerMonteCarloAPI
{
    public static class Utilities
    {
        public static List<Player> GeneratePlayers(IEnumerable<Card> sharedTableCards, List<Card> deckCards,
            Request request)
        {
            return request
                .Players
                .Select(playerRequest => GeneratePlayer(sharedTableCards, deckCards, playerRequest))
                .ToList();
        } 
        public static Player GeneratePlayer(IEnumerable<Card> sharedTableCards, List<Card> deckCards, PlayerRequest playerRequests)
        {
            var allPlayerCards = sharedTableCards.Concat(playerRequests.Cards).ToList();
            while (allPlayerCards.Count < 7)
            {
                allPlayerCards.Add(deckCards.Pop());
            }

            return new Player(allPlayerCards, playerRequests.Folded);
        }
        
        public static List<Card> GenerateTableCards(Request request, List<Card> deckCards)
        {
            var allTableCards = new List<Card>(request.TableCards);

            while (allTableCards.Count < 5)
            {
                allTableCards.Add(deckCards.Pop());
            }

            return allTableCards;
        }
        
        public static IEnumerable<Card> GenerateAllCards()
        {
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Value value in Enum.GetValues(typeof(Value)))
                {
                    if (value != Value.LowAce)
                    {
                        yield return new Card(value, suit);
                    }
                }
            }
        }
        
        public static List<T> FisherYatesShuffle<T>(this List<T> listToShuffle, IRandomService randomService)
        {
            var n = listToShuffle.Count;
            
            while (n > 1)
            {
                var k = randomService.Next(0, n--);
                (listToShuffle[n], listToShuffle[k]) = (listToShuffle[k], listToShuffle[n]);
            }

            return listToShuffle;
        }
        
        public static T Pop<T>(this List<T> list)
        {
            var r = list[^1];
            list.RemoveAt(list.Count - 1);
            return r;
        }
    }
}