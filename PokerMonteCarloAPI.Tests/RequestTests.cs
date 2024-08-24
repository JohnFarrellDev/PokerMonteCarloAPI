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
        private readonly RequestValidator _validator = new();
        private readonly Random _random = new();
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

        private static IEnumerable<(int players, string errorMessage)> InvalidPlayerCounts => new[]
        {
            (0, "Must provide at least one player who has not folded\n'Players Count' must be greater than or equal to '2'.\nPlayers list cannot be empty"),
            (1, "'Players Count' must be greater than or equal to '2'."),
            (15, "'Players Count' must be less than or equal to '14'.")
        };
        [TestCaseSource(nameof(InvalidPlayerCounts))]
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
            
            var request = new Request
            {
                TableCards = TestUtilities.GenerateTableCards(_allCards, _random).ToList(),
                Players = players
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.Errors.Should().ContainSingle(error => 
                error.ErrorMessage == "Each player must have exactly 2 cards");
        }

        [Test]
        public void ValidationFailsWhenAPlayerHasDuplicateCards()
        {
            var players = TestUtilities.GenerateTestPlayers(_allCards, _random).ToList();
            var sharedCard = _allCards.Pop();
            players[0].Cards = new List<Card> { sharedCard, sharedCard };

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
            players[0].Cards = new List<Card> { sharedCard, _allCards.Pop() };
            var tableCards = new List<Card> { sharedCard };
            
            var request = new Request
            {
                TableCards = tableCards,
                Players = players
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("Player cards must not be present in the table cards");
        }

        [Test]
        public void ValidationFailsWhenCardValueIsNotValidForAnyPlayerCards()
        {
            var players = TestUtilities.GenerateTestPlayers(_allCards, _random).ToList();
            players[0].Cards = new List<Card> { new Card((Value)0, Suit.Diamonds) };

            var request = new Request
            {
                TableCards = TestUtilities.GenerateTableCards(_allCards, _random).ToList(),
                Players = players
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("Invalid card value: 0. Allowed values are: LowAce, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace");
        }

        [Test]
        public void ValidationFailsWhenCardSuitIsNotValidForAnyPlayerCards()
        {
            var players = TestUtilities.GenerateTestPlayers(_allCards, _random).ToList();
            players[0].Cards = new List<Card> { new Card(Value.Ace, (Suit)4) };

            var request = new Request
            {
                TableCards = TestUtilities.GenerateTableCards(_allCards, _random).ToList(),
                Players = players
            };

            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("Invalid card suit: 4. Allowed suits are: Clubs, Spades, Diamonds, Hearts");
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
                TableCards = new List<Card> { new Card((Value)15, Suit.Hearts) },
                Players = TestUtilities.GenerateTestPlayers(_allCards, _random).ToList()
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be($"Invalid card value: 15. Allowed values are: LowAce, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace");
        }

        [Test]
        public void ValidationFailsWhenCardSuitIsNotValidForAnyTableCards()
        {
            var request = new Request
            {
                TableCards = new List<Card> { new Card(Value.Ace, (Suit)4) },
                Players = TestUtilities.GenerateTestPlayers(_allCards, _random).ToList()
            };

            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("Invalid card suit: 4. Allowed suits are: Clubs, Spades, Diamonds, Hearts");
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