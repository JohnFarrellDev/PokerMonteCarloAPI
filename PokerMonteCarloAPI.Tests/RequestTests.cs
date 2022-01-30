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
        private readonly TestUtilities _testUtilities = new TestUtilities();
        private List<Card> allCards = null!;
        
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
            var tableCards = _testUtilities.GenerateTableCards(allCards, gameStage).ToList();
            var players = _testUtilities.GeneratePlayers(allCards).ToList();
            
            var request = new Request
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