using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;

#nullable enable
namespace PokerMonteCarloAPI.Tests
{
    public class TestUtilities
    {
        private readonly Random _random = new Random();
        private readonly Faker _faker = new Faker();
        
        public IEnumerable<Card> GenerateTableCards(List<Card> allCards, GameStage gameStage)
        {
            for(var i = 0; i < Constants.MapGameStageToExpectedTableCards[gameStage]; i++)
            {
                yield return allCards.Pop();
            }
        }

        public IEnumerable<PlayerRequest> GeneratePlayers(List<Card> allCards)
        {
            var numberOfPlayers = _random.Next(13) + 2;
            
            for (var i = 0; i < numberOfPlayers; i++)
            {
                yield return GeneratePlayerRequest(allCards, _random.Next(3), _faker.PickRandom(true, false));
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