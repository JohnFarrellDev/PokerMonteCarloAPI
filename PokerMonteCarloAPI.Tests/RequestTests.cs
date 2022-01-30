using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using FluentAssertions;

#nullable enable
namespace PokerMonteCarloAPI.Tests
{
    [TestFixture]
    public class RequestTests
    {
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
            var tableCards = _testUtilities.GenerateTableCards(allCards).ToList();
            var players = _testUtilities.GeneratePlayers(allCards).ToList();
            
            var request = new Request
            {
                TableCards = tableCards,
                Players = players
            };

            var validator = new RequestValidator();
            var validationResults = validator.Validate(request);

            validationResults.IsValid.Should().BeTrue();
        }

        private static object[] invalidPlayerCounts =
        {
            new object[] {0, "'Players Count' must be greater than or equal to '2'."},
            new object[] {1, "'Players Count' must be greater than or equal to '2'."},
            new object[] {15, "'Players Count' must be less than or equal to '14'."},
        };
        [TestCaseSource(nameof(invalidPlayerCounts))]
        public void ValidationFailsWhenLessThan2PlayersOrMoreThan14(int numberOfPlayers, string errorMessage)
        {
            var tableCards = _testUtilities.GenerateTableCards(allCards).ToList();
            var players = _testUtilities.GeneratePlayers(allCards, numberOfPlayers).ToList();
            
            var request = new Request
            {
                TableCards = tableCards,
                Players = players
            };

            var validator = new RequestValidator();
            var validationResults = validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be(errorMessage);
        }
        
        [Test]
        public void ValidationFailsWhenPlayerSubmittedWithMoreThan2Cards()
        {
            var tableCards = _testUtilities.GenerateTableCards(allCards).ToList();
            var players = _testUtilities.GeneratePlayers(allCards).ToList();
            players[0].Cards.Add(allCards.Pop());
            players[0].Cards.Add(allCards.Pop());
            players[0].Cards.Add(allCards.Pop());
            
            var request = new Request
            {
                TableCards = tableCards,
                Players = players
            };

            var validator = new RequestValidator();
            var validationResults = validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("can only provide at most 2 cards for any player");
        }

        [Test]
        public void ValidationFailsWhenAPlayerHasDuplicateCards()
        {
            var tableCards = _testUtilities.GenerateTableCards(allCards).ToList();
            var players = _testUtilities.GeneratePlayers(allCards).ToList();
            players[0].Cards = new List<Card>
            {
                new Card(Value.King, Suit.Hearts),
                new Card(Value.King, Suit.Hearts)
            };

            var request = new Request
            {
                TableCards = tableCards,
                Players = players
            };

            var validator = new RequestValidator();
            var validationResults = validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("provided cards (player and table cards) must all be unique");
        }
        
        [Test]
        public void ValidationFailsWhenADifferentPlayersShareACard()
        {
            var tableCards = _testUtilities.GenerateTableCards(allCards).ToList();
            var players = _testUtilities.GeneratePlayers(allCards).ToList();
            players[0].Cards = new List<Card>
            {
                new Card(Value.King, Suit.Hearts)
            };
            players[1].Cards = new List<Card>
            {
                new Card(Value.King, Suit.Hearts)
            };

            var request = new Request
            {
                TableCards = tableCards,
                Players = players
            };

            var validator = new RequestValidator();
            var validationResults = validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("provided cards (player and table cards) must all be unique");
        }
        
        [Test]
        public void ValidationFailsWhenAPlayersSharesACardWithTheTable()
        {
            var tableCards = _testUtilities.GenerateTableCards(allCards).ToList();
            var players = _testUtilities.GeneratePlayers(allCards).ToList();
            players[0].Cards = new List<Card>
            {
                new Card(Value.King, Suit.Hearts)
            };

            var request = new Request
            {
                TableCards = tableCards,
                Players = players
            };

            var validator = new RequestValidator();
            var validationResults = validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("provided cards (player and table cards) must all be unique");
        }

        [Test]
        public void ValidationFailsWhenCardValueIsNotValid()
        {
            
        }

        [Test]
        public void ValidationFailsWhenCardSuitIsNotValid()
        {
            
        }
        // test each property conditions, show fails as expected for every fail possibility
        // test with faker, run text x1000 for fuzzing? should all pass
    }
}