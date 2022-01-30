using System;
using System.Collections.Generic;

#nullable enable
namespace PokerMonteCarloAPI
{
    public static class Utilities
    {
        public static List<Card> GenerateTableCards(Request request, List<Card> deckCards)
        {
            var allTableCards = new List<Card>(request.TableCards);

            while (allTableCards.Count < 5)
            {
                allTableCards.Add(deckCards.Pop());
            }

            return allTableCards;
        }

        // public static Player GeneratePlayer()
        // {
        //     
        // }
        
        public static IEnumerable<Card> GenerateAllCards()
        {
            foreach (var suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (var value in Enum.GetValues(typeof(Value)))
                {
                    yield return new Card((Value)value, (Suit) suit);
                }
            }
        }

        public static List<T> FisherYatesShuffle<T>(this List<T> listToShuffle)
        {
            var random = new Random();
            var n = listToShuffle.Count;

            while (n > 1)
            {
                var k = random.Next(n--);
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