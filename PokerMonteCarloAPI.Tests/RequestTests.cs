using System.Collections.Generic;
using Bogus;
using NUnit.Framework;
using FluentAssertions;

#nullable enable
namespace PokerMonteCarloAPI.Tests
{
    [TestFixture]
    public class RequestTests
    {
        private Faker _faker = new Faker();
        
        [SetUp]
        public void Setup()
        {
            
        }

        // we want to run test x1000
        // we need to make a pack of cards and apply them randomly but remove, this is to not have any duplicates
        // we need to choose a game stage and correctly put number of table cards down
        
        [Test]
        public void ValidationPassedWithValidProperties()
        {
            var request = new Request()
            {
                GameStage = GameStage.Flop,
                TableCards = new List<Card>(),
                Players = new List<PlayerRequests>
                {
                    new PlayerRequests
                    {
                        Cards = new List<Card>(),
                        Folded = true,
                    },
                    new PlayerRequests
                    {
                        Cards = new List<Card>(),
                        Folded = true,
                    }
                }
            };

            var validator = new RequestValidator();
            var validationResults = validator.Validate(request);

            validationResults.IsValid.Should().BeTrue();
        }
        
        // test each property conditions, show fails as expected for every fail possibility
        // test with faker, run text x1000 for fuzzing? should all pass
    }
}