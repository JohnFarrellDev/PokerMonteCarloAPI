using System;
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
        private readonly RequestValidator _validator = new RequestValidator();
        private readonly Random _random = new Random();
        private List<Card> allCards = null!;
        
        [SetUp]
        public void Setup()
        {
            allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle();
        }
        
        [Test]
        [Repeat(1000)]
        public void ValidationPassedWithValidProperties()
        {
            var request = TestUtilities.GenerateRequest(allCards, _random);
            
            var validationResults = _validator.Validate(request);

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
            var request = TestUtilities.GenerateRequest(allCards, _random, numberOfPlayers);
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be(errorMessage);
        }
        
        [Test]
        public void ValidationFailsWhenPlayerSubmittedWithMoreThan2Cards()
        {
            var players = TestUtilities.GenerateTestPlayers(allCards, _random).ToList();
            players[0].Cards.Add(allCards.Pop());
            players[0].Cards.Add(allCards.Pop());
            players[0].Cards.Add(allCards.Pop());
            
            var request = new Request
            {
                TableCards = TestUtilities.GenerateTableCards(allCards, _random).ToList(),
                Players = players
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("can only provide at most 2 cards for any player");
        }

        [Test]
        public void ValidationFailsWhenAPlayerHasDuplicateCards()
        {
            var players = TestUtilities.GenerateTestPlayers(allCards, _random).ToList();
            var sharedCard = allCards.Pop();
            players[0].Cards = new List<Card> { sharedCard, sharedCard, };

            var request = new Request
            {
                TableCards = TestUtilities.GenerateTableCards(allCards, _random).ToList(),
                Players = players
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("provided cards (player and table cards) must all be unique");
        }
        
        [Test]
        public void ValidationFailsWhenDifferentPlayersShareACard()
        {
            var players = TestUtilities.GenerateTestPlayers(allCards, _random).ToList();
            var sharedCard = allCards.Pop();
            players[0].Cards = new List<Card> { sharedCard };
            players[1].Cards = new List<Card> { sharedCard };

            var request = new Request
            {
                TableCards = TestUtilities.GenerateTableCards(allCards, _random).ToList(),
                Players = players
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("provided cards (player and table cards) must all be unique");
        }
        
        [Test]
        public void ValidationFailsWhenAPlayersSharesACardWithTheTable()
        {
            var players = TestUtilities.GenerateTestPlayers(allCards, _random).ToList();
            var sharedCard = allCards.Pop();
            players[0].Cards = new List<Card> { sharedCard };
            var tableCards = new List<Card> { sharedCard };
            
            var request = new Request
            {
                TableCards = tableCards,
                Players = players
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("provided cards (player and table cards) must all be unique");
        }

        [Test]
        public void ValidationFailsWhenCardValueIsNotValidForAnyPlayerCards()
        {
            var players = TestUtilities.GenerateTestPlayers(allCards, _random).ToList();
            players[0].Cards = new List<Card> { new Card((Value) 15, Suit.Diamonds) };

            var request = new Request
            {
                TableCards = TestUtilities.GenerateTableCards(allCards, _random).ToList(),
                Players = players
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("card value must be between 2 and 14 (inclusive), card suit must be between 0 and 3 (inclusive)");
        }

        [Test]
        public void ValidationFailsWhenCardSuitIsNotValidForAnyPlayerCards()
        {
            var players = TestUtilities.GenerateTestPlayers(allCards, _random).ToList();
            players[0].Cards = new List<Card> { new Card(Value.Ace, (Suit) 4) };

            var request = new Request
            {
                TableCards = TestUtilities.GenerateTableCards(allCards, _random).ToList(),
                Players = players
            };

            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("card value must be between 2 and 14 (inclusive), card suit must be between 0 and 3 (inclusive)");
        }

        [Test]
        public void ValidationFailsWhenMoreThan5TableCardsProvided()
        {
            var tableCards = new List<Card>
            {
                allCards.Pop(), allCards.Pop(), allCards.Pop(), allCards.Pop(), allCards.Pop(), allCards.Pop(),
            };
            
            var request = new Request
            {
                TableCards = tableCards,
                Players = TestUtilities.GenerateTestPlayers(allCards, _random).ToList()
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("'Table Cards Count' must be less than or equal to '5'.");
        }
        
        [Test]
        public void ValidationFailsWhenCardValueIsNotValidForAnyTableCards()
        {
            var request = new Request
            {
                TableCards = new List<Card> { new Card((Value) 15, Suit.Hearts) },
                Players = TestUtilities.GenerateTestPlayers(allCards, _random).ToList()
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("card value must be between 2 and 14 (inclusive), card suit must be between 0 and 3 (inclusive)");
        }

        [Test]
        public void ValidationFailsWhenCardSuitIsNotValidForAnyTableCards()
        {
            var request = new Request
            {
                TableCards = new List<Card> { new Card(Value.Ace, (Suit) 4) },
                Players = TestUtilities.GenerateTestPlayers(allCards, _random).ToList()
            };

            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("card value must be between 2 and 14 (inclusive), card suit must be between 0 and 3 (inclusive)");
        }
    }
}