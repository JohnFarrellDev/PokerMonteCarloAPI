using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PokerMonteCarloAPI
{
    public static class Utilities
    {
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