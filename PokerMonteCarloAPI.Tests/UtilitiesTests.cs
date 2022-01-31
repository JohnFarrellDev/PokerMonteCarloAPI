using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace PokerMonteCarloAPI.Tests
{
    [TestFixture]
    public class UtilitiesTests
    {
        private readonly TestUtilities _testUtilities = new TestUtilities();
        
        [Test]
        public void GenerateAllCardsProduced52DistinctPlayingCards()
        {
            var allCards = Utilities.GenerateAllCards().ToList();
            var allCardsSet = new HashSet<Card>(allCards);

            allCards.Count.Should().Be(52);
            allCardsSet.Count.Should().Be(52);

            allCardsSet.Count(card => card.suit == Suit.Clubs).Should().Be(13);
            allCardsSet.Count(card => card.suit == Suit.Diamonds).Should().Be(13);
            allCardsSet.Count(card => card.suit == Suit.Hearts).Should().Be(13);
            allCardsSet.Count(card => card.suit == Suit.Spades).Should().Be(13);
            
            allCardsSet.Count(card => card.value == Value.Two).Should().Be(4);
            allCardsSet.Count(card => card.value == Value.Three).Should().Be(4);
            allCardsSet.Count(card => card.value == Value.Four).Should().Be(4);
            allCardsSet.Count(card => card.value == Value.Five).Should().Be(4);
            allCardsSet.Count(card => card.value == Value.Six).Should().Be(4);
            allCardsSet.Count(card => card.value == Value.Seven).Should().Be(4);
            allCardsSet.Count(card => card.value == Value.Eight).Should().Be(4);
            allCardsSet.Count(card => card.value == Value.Nine).Should().Be(4);
            allCardsSet.Count(card => card.value == Value.Ten).Should().Be(4);
            allCardsSet.Count(card => card.value == Value.Jack).Should().Be(4);
            allCardsSet.Count(card => card.value == Value.Queen).Should().Be(4);
            allCardsSet.Count(card => card.value == Value.King).Should().Be(4);
            allCardsSet.Count(card => card.value == Value.Ace).Should().Be(4);
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

        [Test]
        public void FisherYatesShouldRandomlyShuffleOurListOf52PlayingCards()
        {
            var countCardShuffledPosition = new Dictionary<Card, int>();
            
            for (var i = 0; i < 10_000; i++)
            {
                var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle();
                for (var j = 0; j < allCards.Count; j++)
                {
                    var card = allCards[j];
                    countCardShuffledPosition[card] =
                        countCardShuffledPosition.TryGetValue(card, out var value) ? value + j : j;
                }
            }

            var average = countCardShuffledPosition.Sum(x => x.Value)/countCardShuffledPosition.Count;
            var deviationSquared = countCardShuffledPosition.Sum(x => Math.Pow(x.Value - average, 2));
            var variance = deviationSquared / countCardShuffledPosition.Count;
            var standardDeviation = Math.Sqrt(variance);

            // standard deviation of 0 means perfect "randomness"
            // standard deviation of 150,000 when fisher-yates not applied
            // standard deviation typically between 1,300-1,600 with fisher-yates
            standardDeviation.Should().BeLessThan(2_000);
        }

        [Test]
        public void GenerateAllTableCardsFromRequest()
        {
            // arrange
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle();
            
            var request = _testUtilities.GenerateRequest(allCards);
            while (request.TableCards.Count < 5)
            {
                request.TableCards.Add(allCards.Pop());
            }

            // act
            var tableCards = Utilities.GenerateTableCards(request, allCards);

            // assert
            tableCards.Count.Should().Be(5);
            for (var i = 0; i < 5; i++)
            {
                tableCards[i].Should().BeEquivalentTo(request.TableCards[i]);
            }
        }

        [Test]
        public void GenerateAllTableCardsFromDeck()
        {
            // arrange
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle();
            var request = _testUtilities.GenerateRequest(allCards);
            request.TableCards = new List<Card>();
            var top5CardsFromDeck = allCards.TakeLast(5).ToList();
            
            // act
            var tableCards = Utilities.GenerateTableCards(request, allCards);

            // assert
            tableCards.Count.Should().Be(5);
            for (var i = 0; i < 5; i++)
            {
                tableCards[i].Should().BeEquivalentTo(top5CardsFromDeck[tableCards.Count - 1 - i]);
            }
        }

        [Test]
        public void GenerateTableCardsFromRequestAndDeck()
        {
            // arrange
            const int numberOfCardsFromRequests = 3;
            const int numberOfCardsFromDeck = 2;
            
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle();
            var request = _testUtilities.GenerateRequest(allCards);
            request.TableCards = new List<Card>();
            
            while (request.TableCards.Count < numberOfCardsFromRequests)
            {
                request.TableCards.Add(allCards.Pop());
            }

            var cardsFromDeck = allCards.TakeLast(numberOfCardsFromDeck).ToList();
            
            // act
            var tableCards = Utilities.GenerateTableCards(request, allCards);
            
            // assert
            tableCards.Count.Should().Be(5);
            
            for (var i = 0; i < numberOfCardsFromRequests; i++)
            {
                tableCards[i].Should().BeEquivalentTo(request.TableCards[i]);
            }
            
            tableCards.Reverse();
            for (var i = 0; i < numberOfCardsFromDeck; i++)
            {
                tableCards[i].Should().BeEquivalentTo(cardsFromDeck[i]);
            }
        }

        [Test]
        public void TestGeneratePlayerWithFullHandAndTableCards()
        {
            // arrange
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle();
            var request = _testUtilities.GenerateRequest(allCards);
            var sharedTableCards = Utilities.GenerateTableCards(request, allCards);

            var playerRequest = request.Players[0];
            while (playerRequest.Cards.Count < 2)
            {
                playerRequest.Cards.Add(allCards.Pop());
            }
            
            var preGeneratingPlayerAllCardsCount = allCards.Count;

            // act
            var generatedPlayer = Utilities.GeneratePlayer(sharedTableCards, allCards, playerRequest);
            
            // assert
            generatedPlayer.playersHand.Count.Should().Be(7);
            allCards.Count.Should().Be(preGeneratingPlayerAllCardsCount);
            
            foreach (var sharedTableCard in sharedTableCards)
            {
                generatedPlayer.playersHand.Contains(sharedTableCard).Should().BeTrue();
            }
            foreach (var playerRequestCard in playerRequest.Cards)
            {
                generatedPlayer.playersHand.Contains(playerRequestCard).Should().BeTrue();
            }
        }

        [Test]
        public void TestGeneratePlayerWithNoTableCards()
        {
            // arrange
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle();
            var request = _testUtilities.GenerateRequest(allCards);
            request.TableCards = new List<Card>();
            var deckCardsToBeAdded = allCards.TakeLast(5);

            var playerRequest = request.Players[0];
            while (playerRequest.Cards.Count < 2)
            {
                playerRequest.Cards.Add(allCards.Pop());
            }
            
            var preGeneratingPlayerAllCardsCount = allCards.Count;

            // act
            var generatedPlayer = Utilities.GeneratePlayer(new List<Card>(), allCards, playerRequest);
            
            // assert
            generatedPlayer.playersHand.Count.Should().Be(7);
            allCards.Count.Should().Be(preGeneratingPlayerAllCardsCount - 5);
            
            foreach (var playerRequestCard in playerRequest.Cards)
            {
                generatedPlayer.playersHand.Contains(playerRequestCard).Should().BeTrue();
            }
            foreach (var deckCardToBeAdded in deckCardsToBeAdded)
            {
                generatedPlayer.playersHand.Contains(deckCardToBeAdded).Should().BeTrue();
            }
        }
        
        [Test]
        public void TestGeneratePlayerWithNoProvidedPlayerCards()
        {
            // arrange
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle();
            var request = _testUtilities.GenerateRequest(allCards);
            request.Players[0].Cards = new List<Card>();
            var sharedTableCards = Utilities.GenerateTableCards(request, allCards);
            
            var cardsToBeAppliedToPlayer = allCards.TakeLast(2);
            
            var preGeneratingPlayerAllCardsCount = allCards.Count;

            // act
            var generatedPlayer = Utilities.GeneratePlayer(sharedTableCards, allCards, request.Players[0]);
            
            // assert
            generatedPlayer.playersHand.Count.Should().Be(7);
            allCards.Count.Should().Be(preGeneratingPlayerAllCardsCount - 2);
            
            foreach (var sharedTableCard in sharedTableCards)
            {
                generatedPlayer.playersHand.Contains(sharedTableCard).Should().BeTrue();
            }
            foreach (var deckCardToBeAdded in cardsToBeAppliedToPlayer)
            {
                generatedPlayer.playersHand.Contains(deckCardToBeAdded).Should().BeTrue();
            }
        }
    }
}