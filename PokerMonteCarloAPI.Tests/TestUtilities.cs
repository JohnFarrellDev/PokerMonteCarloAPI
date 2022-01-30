using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace PokerMonteCarloAPI.Tests
{
    public class TestUtilities
    {
        private readonly Random _random = new Random();

        public IEnumerable<Card> GenerateTableCards(List<Card> allCards)
        {
            for(var i = 0; i < _random.Next(6); i++)
            {
                yield return allCards.Pop();
            }
        }

        public IEnumerable<PlayerRequest> GeneratePlayers(List<Card> allCards, int? numberOfPlayers = null)
        {
            numberOfPlayers ??= _random.Next(13) + 2;
            
            for (var i = 0; i < numberOfPlayers; i++)
            {
                yield return GeneratePlayerRequest(allCards, _random.Next(3), _random.Next(2) == 1);
            }
        }
        
        private static PlayerRequest GeneratePlayerRequest(List<Card> allCards, int numberOfCards, bool folded)
        {
            return new PlayerRequest
            {
                Cards = GeneratePlayerCards(allCards, numberOfCards).ToList(),
                Folded = folded
            };
        }

        private static IEnumerable<Card> GeneratePlayerCards(List<Card> allCards, int numberOfCards)
        {
            for (var i = 0; i < numberOfCards; i++)
            {
                yield return allCards.Pop();
            }
        }
    }
}