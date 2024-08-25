using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using PokerMonteCarloAPI.Services;
using Moq;

#nullable enable
namespace PokerMonteCarloAPI.Tests
{
    [TestFixture]
    public class UtilitiesTests
    {
        private Mock<IRandomService> _mockRandomService = null!;
        
        [SetUp]
        public void Setup()
        {
            _mockRandomService = new Mock<IRandomService>();
        }
        
        [Test]
        public void GenerateAllCardsProduced52DistinctPlayingCards()
        {
            var allCards = Utilities.GenerateAllCards().ToList();
            var allCardsSet = new HashSet<Card>(allCards);

            allCards.Count.Should().Be(52);
            allCardsSet.Count.Should().Be(52);

            allCardsSet.Count(card => card.Suit == Suit.Hearts).Should().Be(13);
            allCardsSet.Count(card => card.Suit == Suit.Diamonds).Should().Be(13);
            allCardsSet.Count(card => card.Suit == Suit.Clubs).Should().Be(13);
            allCardsSet.Count(card => card.Suit == Suit.Spades).Should().Be(13);
            
            allCardsSet.Count(card => card.Value == Value.Two).Should().Be(4);
            allCardsSet.Count(card => card.Value == Value.Three).Should().Be(4);
            allCardsSet.Count(card => card.Value == Value.Four).Should().Be(4);
            allCardsSet.Count(card => card.Value == Value.Five).Should().Be(4);
            allCardsSet.Count(card => card.Value == Value.Six).Should().Be(4);
            allCardsSet.Count(card => card.Value == Value.Seven).Should().Be(4);
            allCardsSet.Count(card => card.Value == Value.Eight).Should().Be(4);
            allCardsSet.Count(card => card.Value == Value.Nine).Should().Be(4);
            allCardsSet.Count(card => card.Value == Value.Ten).Should().Be(4);
            allCardsSet.Count(card => card.Value == Value.Jack).Should().Be(4);
            allCardsSet.Count(card => card.Value == Value.Queen).Should().Be(4);
            allCardsSet.Count(card => card.Value == Value.King).Should().Be(4);
            allCardsSet.Count(card => card.Value == Value.Ace).Should().Be(4);
        }

        [Test]
        public void CallingPopReturnsTheLastElementFromAListAndAlsoRemovesTheElementFromTheList()
        {
            var elements = new List<int> { 1, 2, 3, 4, 5 };
            var poppedElement = elements.Pop();

            poppedElement.Should().Be(5);
            elements.Count.Should().Be(4);
            elements.Contains(5).Should().BeFalse();
        }

        private static object[] _seededFisherYatesShuffle =
        {
            new object[] {1, new List<int> {5,4,1,3,2}},
            new object[] {11, new List<int> {3,4,5,1,2}},
            new object[] {111, new List<int> {5,4,3,1,2}},
            new object[] {1111, new List<int> {5,3,4,1,2}},
            new object[] {11111, new List<int> {3,4,5,1,2}},
            new object[] {2, new List<int> {1,5,2,4,3}},
            new object[] {22, new List<int> {5,2,1,4,3}},
            new object[] {222, new List<int> {1,2,5,4,3}},
            new object[] {2222, new List<int> {1,5,2,4,3}},
            new object[] {22222, new List<int> {5,2,1,4,3}},
            new object[] {222222, new List<int> {1,2,5,4,3}},
            new object[] {3, new List<int> {2,5,3,1,4}},
            new object[] {33, new List<int> {2,1,5,3,4}},
            new object[] {333, new List<int> {2,1,5,3,4}},
            new object[] {3333, new List<int> {2,1,5,3,4}},
            new object[] {33333, new List<int> {2,1,5,3,4}},
            new object[] {333333, new List<int> {2,1,5,3,4}}
        };
        [TestCaseSource(nameof(_seededFisherYatesShuffle))]
        public void FisherYatesWhenRandomSeededProducesSameOutput(int seed, List<int> expectedOutput)
        {
            int currentIndex = 0;
            _mockRandomService.Setup(r => r.Next(It.IsAny<int>(), It.IsAny<int>()))
                .Returns((int min, int max) => 
                {
                    int result = (seed + currentIndex) % (max - min) + min;
                    currentIndex++;
                    return result;
                });

            var shuffledNumbers1To5 = Enumerable.Range(1, 5).ToList().FisherYatesShuffle(_mockRandomService.Object);

            shuffledNumbers1To5.Should().BeEquivalentTo(expectedOutput, options => options.WithStrictOrdering());
        }
        
        [Test]
        public void GenerateAllTableCardsFromRequest()
        {
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle(_mockRandomService.Object);
            
            var request = TestUtilities.GenerateRequest(allCards, _mockRandomService.Object);
            while (request.TableCards.Count < 5)
            {
                request.TableCards.Add(allCards.Pop());
            }

            var tableCards = Utilities.GenerateTableCards(request, allCards);

            tableCards.Count.Should().Be(5);
            tableCards.Should().ContainInOrder(request.TableCards);
        }

        [Test]
        public void GenerateAllTableCardsFromDeck()
        {
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle(_mockRandomService.Object);
            var request = TestUtilities.GenerateRequest(allCards, _mockRandomService.Object);
            request.TableCards = new List<Card>();
            var top5CardsFromDeck = allCards.TakeLast(5).ToList();
            top5CardsFromDeck.Reverse();
            
            var tableCards = Utilities.GenerateTableCards(request, allCards);

            tableCards.Count.Should().Be(5);
            tableCards.Should().ContainInOrder(top5CardsFromDeck);
        }

        [Test]
        public void GenerateTableCardsFromRequestAndDeck()
        {
            const int numberOfCardsFromRequests = 3;
            const int numberOfCardsFromDeck = 2;
            
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle(_mockRandomService.Object);
            var request = TestUtilities.GenerateRequest(allCards, _mockRandomService.Object);
            request.TableCards = new List<Card>();
            
            while (request.TableCards.Count < numberOfCardsFromRequests)
            {
                request.TableCards.Add(allCards.Pop());
            }

            var cardsFromDeck = allCards.TakeLast(numberOfCardsFromDeck).ToList();
            cardsFromDeck.Reverse();
            
            var tableCards = Utilities.GenerateTableCards(request, allCards);
            
            tableCards.Count.Should().Be(5);
            tableCards.Should().ContainInOrder(request.TableCards.Concat(cardsFromDeck));
        }

        [Test]
        public void TestGeneratePlayerWithFullHandAndTableCards()
        {
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle(_mockRandomService.Object);
            var request = TestUtilities.GenerateRequest(allCards, _mockRandomService.Object);
            var sharedTableCards = Utilities.GenerateTableCards(request, allCards);

            var playerRequest = request.Players[0];
            while (playerRequest.Cards.Count < 2)
            {
                playerRequest.Cards.Add(allCards.Pop());
            }
            
            var preGeneratingPlayerAllCardsCount = allCards.Count;

            var generatedPlayer = Utilities.GeneratePlayer(sharedTableCards, allCards, playerRequest);
            
            generatedPlayer.PlayersHand.Count.Should().Be(7);
            allCards.Count.Should().Be(preGeneratingPlayerAllCardsCount);
            
            foreach (var sharedTableCard in sharedTableCards)
            {
                generatedPlayer.PlayersHand.Contains(sharedTableCard).Should().BeTrue();
            }
            foreach (var playerRequestCard in playerRequest.Cards)
            {
                generatedPlayer.PlayersHand.Contains(playerRequestCard).Should().BeTrue();
            }
        }

        [Test]
        public void TestGeneratePlayerWithNoTableCards()
        {
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle(_mockRandomService.Object);
            var request = TestUtilities.GenerateRequest(allCards, _mockRandomService.Object);
            request.TableCards = new List<Card>();
            var deckCardsToBeAdded = allCards.TakeLast(5).ToList();

            var playerRequest = request.Players[0];
            while (playerRequest.Cards.Count < 2)
            {
                playerRequest.Cards.Add(allCards.Pop());
            }
            
            var preGeneratingPlayerAllCardsCount = allCards.Count;

            var generatedPlayer = Utilities.GeneratePlayer(new List<Card>(), allCards, playerRequest);
            
            generatedPlayer.PlayersHand.Count.Should().Be(7);
            allCards.Count.Should().Be(preGeneratingPlayerAllCardsCount - 5);
            
            foreach (var playerRequestCard in playerRequest.Cards)
            {
                generatedPlayer.PlayersHand.Contains(playerRequestCard).Should().BeTrue();
            }
            foreach (var deckCardToBeAdded in deckCardsToBeAdded)
            {
                generatedPlayer.PlayersHand.Contains(deckCardToBeAdded).Should().BeTrue();
            }
        }
        
        [Test]
        public void TestGeneratePlayerWithNoProvidedPlayerCards()
        {
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle(_mockRandomService.Object);
            var request = TestUtilities.GenerateRequest(allCards, _mockRandomService.Object);
            request.Players[0].Cards = new List<Card>();
            var sharedTableCards = Utilities.GenerateTableCards(request, allCards);
            
            var cardsToBeAppliedToPlayer = allCards.TakeLast(2).ToArray();
            
            var preGeneratingPlayerAllCardsCount = allCards.Count;

            var generatedPlayer = Utilities.GeneratePlayer(sharedTableCards, allCards, request.Players[0]);
            
            generatedPlayer.PlayersHand.Count.Should().Be(7);
            allCards.Count.Should().Be(preGeneratingPlayerAllCardsCount - 2);
            
            foreach (var sharedTableCard in sharedTableCards)
            {
                generatedPlayer.PlayersHand.Contains(sharedTableCard).Should().BeTrue();
            }
            foreach (var deckCardToBeAdded in cardsToBeAppliedToPlayer)
            {
                generatedPlayer.PlayersHand.Contains(deckCardToBeAdded).Should().BeTrue();
            }
        }

        [Test]
        public void TestGeneratePlayersTakeARequestAndOutputsAListOfValidPlayers()
        {
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle(_mockRandomService.Object);
            var request = TestUtilities.GenerateRequest(allCards, _mockRandomService.Object);
            var sharedTableCards = Utilities.GenerateTableCards(request, allCards);

            var generatedPlayers = Utilities.GeneratePlayers(sharedTableCards, allCards, request);

            generatedPlayers.Count.Should().Be(request.Players.Count);
            
            for (var i = 0; i < generatedPlayers.Count; i++)
            {
                var player = generatedPlayers[i];
                var playerRequest = request.Players[i];
                
                foreach (var playerRequestCard in playerRequest.Cards)
                {
                    player.PlayersHand.Contains(playerRequestCard).Should().BeTrue();
                }
            }
            
            foreach (var generatedPlayer in generatedPlayers)
            {
                foreach (var sharedTableCard in sharedTableCards)
                {
                    generatedPlayer.PlayersHand.Contains(sharedTableCard).Should().BeTrue();
                }
            }
        }
    }
}