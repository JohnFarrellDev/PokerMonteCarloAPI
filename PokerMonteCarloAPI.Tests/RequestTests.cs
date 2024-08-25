using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using FluentAssertions;
using Moq;
using PokerMonteCarloAPI.Services;

#nullable enable
namespace PokerMonteCarloAPI.Tests
{
    [TestFixture]
    public class RequestTests
    {
        private readonly RequestValidator _validator = new();
        private Mock<IRandomService> _mockRandomService = null!;
        private List<Card> _allCards = null!;
        
        [SetUp]
        public void Setup()
        {
            _mockRandomService = new Mock<IRandomService>();
            _allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle(_mockRandomService.Object);
        }
        
        [Test]
        [Repeat(1000)]
        public void ValidationPassedWithValidProperties()
        {
            var request = TestUtilities.GenerateRequest(_allCards, _mockRandomService.Object);

            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeTrue();
        }

        private static IEnumerable<object[]> InvalidPlayerCounts => new[]
        {
            new object[] { 0, "Must provide at least one player who has not folded\r\n'Players Count' must be greater than or equal to '2'.\r\nPlayers list cannot be empty" },
            new object[] { 1, "'Players Count' must be greater than or equal to '2'." },
            new object[] { 15, "'Players Count' must be less than or equal to '14'." }
        };
        [TestCaseSource(nameof(InvalidPlayerCounts))]
        public void ValidationFailsWhenLessThan2PlayersOrMoreThan14(int numberOfPlayers, string errorMessage)
        {
            var request = TestUtilities.GenerateRequest(_allCards, _mockRandomService.Object, numberOfPlayers);
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be(errorMessage);
        }
        
        [Test]
        public void ValidationFailsWhenPlayerSubmittedWithMoreThan2Cards()
        {
            var players = TestUtilities.GenerateTestPlayers(_allCards, _mockRandomService.Object).ToList();
            players[0].Cards.Add(_allCards.Pop());
            
            var request = new Request
            {
                TableCards = TestUtilities.GenerateTableCards(_allCards, _mockRandomService.Object).ToList(),
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
            var players = TestUtilities.GenerateTestPlayers(_allCards, _mockRandomService.Object).ToList();
            var sharedCard = _allCards.Pop();
            players[0].Cards = new List<Card> { sharedCard, sharedCard };

            var request = new Request
            {
                TableCards = TestUtilities.GenerateTableCards(_allCards, _mockRandomService.Object).ToList(),
                Players = players
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("Players cannot share cards");
        }
        
        [Test]
        public void ValidationFailsWhenDifferentPlayersShareACard()
        {
            var players = TestUtilities.GenerateTestPlayers(_allCards, _mockRandomService.Object).ToList();
            var sharedCard = _allCards.Pop();
            players[0].Cards = new List<Card> { sharedCard, _allCards.Pop() };
            players[1].Cards = new List<Card> { sharedCard, _allCards.Pop() };

            var request = new Request
            {
                TableCards = TestUtilities.GenerateTableCards(_allCards, _mockRandomService.Object).ToList(),
                Players = players
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("Players cannot share cards");
        }
        
        [Test]
        public void ValidationFailsWhenAPlayersSharesACardWithTheTable()
        {
            var players = TestUtilities.GenerateTestPlayers(_allCards, _mockRandomService.Object).ToList();
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
            var players = TestUtilities.GenerateTestPlayers(_allCards, _mockRandomService.Object).ToList();
            players[0].Cards = new List<Card> { new(0, Suit.Diamonds), _allCards.Pop() };

            var request = new Request
            {
                TableCards = TestUtilities.GenerateTableCards(_allCards, _mockRandomService.Object).ToList(),
                Players = players
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("Invalid card value: 0. Allowed values are: 1 (LowAce), 2 (Two), 3 (Three), 4 (Four), 5 (Five), 6 (Six), 7 (Seven), 8 (Eight), 9 (Nine), 10 (Ten), 11 (Jack), 12 (Queen), 13 (King), 14 (Ace)");
        }

        [Test]
        public void ValidationFailsWhenCardSuitIsNotValidForAnyPlayerCards()
        {
            var players = TestUtilities.GenerateTestPlayers(_allCards, _mockRandomService.Object).ToList();
            players[0].Cards = new List<Card> { new(Value.Ace, 0), _allCards.Pop() };

            var request = new Request
            {
                TableCards = TestUtilities.GenerateTableCards(_allCards, _mockRandomService.Object).ToList(),
                Players = players
            };

            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("Invalid card suit: 0. Allowed suits are: 1 (Clubs), 2 (Spades), 3 (Diamonds), 4 (Hearts)");
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
                Players = TestUtilities.GenerateTestPlayers(_allCards, _mockRandomService.Object).ToList()
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
                TableCards = new List<Card> { new(0, Suit.Hearts) },
                Players = TestUtilities.GenerateTestPlayers(_allCards, _mockRandomService.Object).ToList()
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("Invalid card value: 0. Allowed values are: 1 (LowAce), 2 (Two), 3 (Three), 4 (Four), 5 (Five), 6 (Six), 7 (Seven), 8 (Eight), 9 (Nine), 10 (Ten), 11 (Jack), 12 (Queen), 13 (King), 14 (Ace)");
        }

        [Test]
        public void ValidationFailsWhenCardSuitIsNotValidForAnyTableCards()
        {
            var request = new Request
            {
                TableCards = new List<Card> { new (Value.Ace, 0) },
                Players = TestUtilities.GenerateTestPlayers(_allCards, _mockRandomService.Object).ToList()
            };

            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("Invalid card suit: 0. Allowed suits are: 1 (Clubs), 2 (Spades), 3 (Diamonds), 4 (Hearts)");
        }

        [Test]
        public void ValidationFailsWhenAllPlayersAreFolded()
        {
            var request = new Request
            {
                TableCards = new List<Card>(),
                Players = TestUtilities.GenerateTestPlayers(_allCards, _mockRandomService.Object, 5, 5).ToList()
            };
            
            var validationResults = _validator.Validate(request);

            validationResults.IsValid.Should().BeFalse();
            validationResults.ToString().Should().Be("Must provide at least one player who has not folded");
        }
    }
}