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
        private List<Card> _allCards = null!;
        
        [SetUp]
        public void Setup()
        {
            _allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle();
        }
        
        [Test]
        [Repeat(1000)]
        public void ValidationPassedWithValidProperties()
        {
            var request = TestUtilities.GenerateRequest(_allCards, _random);
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeTrue();
        }

        
        
        private static object[] _invalidPlayerCounts =
        {
            new object[] {0, "'Players Count' must be greater than or equal to '2'.\r\nMust provide at least one player who has not folded"},
            new object[] {1, "'Players Count' must be greater than or equal to '2'."},
            new object[] {15, "'Players Count' must be less than or equal to '14'."},
        };
        [TestCaseSource(nameof(_invalidPlayerCounts))]
        public void ValidationFailsWhenLessThan2PlayersOrMoreThan14(int numberOfPlayers, string errorMessage)
        {
            var request = TestUtilities.GenerateRequest(_allCards, _random, numberOfPlayers);
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be(errorMessage);
        }
        
        [Test]
        public void ValidationFailsWhenPlayerSubmittedWithMoreThan2Cards()
        {
            var players = TestUtilities.GenerateTestPlayers(_allCards, _random).ToList();
            players[0].Cards.Add(_allCards.Pop());
            players[0].Cards.Add(_allCards.Pop());
            players[0].Cards.Add(_allCards.Pop());
            
            var request = new Request
            {
                TableCards = TestUtilities.GenerateTableCards(_allCards, _random).ToList(),
                Players = players
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("can only provide at most 2 cards for any player");
        }

        [Test]
        public void ValidationFailsWhenAPlayerHasDuplicateCards()
        {
            var players = TestUtilities.GenerateTestPlayers(_allCards, _random).ToList();
            var sharedCard = _allCards.Pop();
            players[0].Cards = new List<Card> { sharedCard, sharedCard, };

            var request = new Request
            {
                TableCards = TestUtilities.GenerateTableCards(_allCards, _random).ToList(),
                Players = players
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("provided cards (player and table cards) must all be unique");
        }
        
        [Test]
        public void ValidationFailsWhenDifferentPlayersShareACard()
        {
            var players = TestUtilities.GenerateTestPlayers(_allCards, _random).ToList();
            var sharedCard = _allCards.Pop();
            players[0].Cards = new List<Card> { sharedCard };
            players[1].Cards = new List<Card> { sharedCard };

            var request = new Request
            {
                TableCards = TestUtilities.GenerateTableCards(_allCards, _random).ToList(),
                Players = players
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("provided cards (player and table cards) must all be unique");
        }
        
        [Test]
        public void ValidationFailsWhenAPlayersSharesACardWithTheTable()
        {
            var players = TestUtilities.GenerateTestPlayers(_allCards, _random).ToList();
            var sharedCard = _allCards.Pop();
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
            var players = TestUtilities.GenerateTestPlayers(_allCards, _random).ToList();
            players[0].Cards = new List<Card> { new Card(15, 2) };

            var request = new Request
            {
                TableCards = TestUtilities.GenerateTableCards(_allCards, _random).ToList(),
                Players = players
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("card value must be between 2 and 14 (inclusive), card suit must be between 0 and 3 (inclusive)");
        }

        [Test]
        public void ValidationFailsWhenCardSuitIsNotValidForAnyPlayerCards()
        {
            var players = TestUtilities.GenerateTestPlayers(_allCards, _random).ToList();
            players[0].Cards = new List<Card> { new Card(14, 5) };

            var request = new Request
            {
                TableCards = TestUtilities.GenerateTableCards(_allCards, _random).ToList(),
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
                _allCards.Pop(), _allCards.Pop(), _allCards.Pop(), _allCards.Pop(), _allCards.Pop(), _allCards.Pop(),
            };
            
            var request = new Request
            {
                TableCards = tableCards,
                Players = TestUtilities.GenerateTestPlayers(_allCards, _random).ToList()
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
                TableCards = new List<Card> { new Card(15, 1) },
                Players = TestUtilities.GenerateTestPlayers(_allCards, _random).ToList()
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
                TableCards = new List<Card> { new Card(14, 4) },
                Players = TestUtilities.GenerateTestPlayers(_allCards, _random).ToList()
            };

            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("card value must be between 2 and 14 (inclusive), card suit must be between 0 and 3 (inclusive)");
        }

        [Test]
        public void ValidationFailsWhenAllPlayersAreFolded()
        {
            var request = new Request
            {
                TableCards = new List<Card>(),
                Players = TestUtilities.GenerateTestPlayers(_allCards, _random, 5, 5).ToList()
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("Must provide at least one player who has not folded");
        }
    }
}