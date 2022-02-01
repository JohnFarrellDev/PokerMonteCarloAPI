using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace PokerMonteCarloAPI.Tests
{
    public static class TestUtilities
    {
        public static Request GenerateRequest(List<Card> allCards, Random random, int? numberOfPlayers = null)
        {
            return new Request
            {
                TableCards = GenerateTableCards(allCards, random).ToList(),
                Players = GenerateTestPlayers(allCards, random, numberOfPlayers).ToList()
            };
        }

        public static Response GenerateResponse()
        {
            return new Response
            {
                Id = 1,
                Test = "hello world"
            };
        }
        
        public static IEnumerable<Card> GenerateTableCards(List<Card> allCards, Random random)
        {
            for(var i = 0; i < random.Next(6); i++)
            {
                yield return allCards.Pop();
            }
        }

        public static IEnumerable<PlayerRequest> GenerateTestPlayers(List<Card> allCards, Random random, int? numberOfPlayers = null)
        {
            numberOfPlayers ??= random.Next(13) + 2;
            
            for (var i = 0; i < numberOfPlayers; i++)
            {
                yield return GeneratePlayerRequest(allCards, random.Next(3), random.Next(2) == 1);
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