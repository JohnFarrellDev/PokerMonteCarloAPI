using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using NUnit.Framework;
using FluentAssertions;

#nullable enable
namespace PokerMonteCarloAPI.Tests
{
    [TestFixture]
    public class RequestTests
    {
        private readonly Faker _faker = new Faker();
        private List<Card> allCards = null!;
        private readonly Random _random = new Random();
        
        [SetUp]
        public void Setup()
        {
            allCards = Utilities.GenerateAllCards().ToList();
            Utilities.FisherYatesShuffle(allCards);
        }
        
        [Test]
        [Repeat(1000)]
        public void ValidationPassedWithValidProperties()
        {
            var gameStage = _faker.PickRandom<GameStage>();

            var tableCards = new List<Card>();
            for(var i = 0; i < Constants.MapGameStageToExpectedTableCards[gameStage]; i++)
            {
                tableCards.Add(allCards.Pop());
            }

            var numberOfPlayers = _random.Next(13) + 2;
            var players = new List<PlayerRequests>();
            for (var i = 0; i < numberOfPlayers; i++)
            {
                var player = new PlayerRequests
                {
                    Cards = new List<Card>()
                };
                for (var j = 0; j < _random.Next(3); j++)
                {
                    player.Cards.Add(allCards.Pop());
                }
                player.Folded = _faker.PickRandomParam(true, false);
                
                players.Add(player);
            }
            
            var request = new Request()
            {
                GameStage = gameStage,
                TableCards = tableCards,
                Players = players
            };

            var validator = new RequestValidator();
            var validationResults = validator.Validate(request);

            validationResults.IsValid.Should().BeTrue();
        }
        
        // test each property conditions, show fails as expected for every fail possibility
        // test with faker, run text x1000 for fuzzing? should all pass
    }
}