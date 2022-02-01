using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using ScottPlot;

namespace PokerMonteCarloAPI.Tests
{
    [TestFixture]
    public class UtilitiesTests
    {
        private readonly Random Random = new Random();
        
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
        public void FisherYatesShouldRandomlyShuffleAList()
        {
            // arrange
            const int upperIterationBound = 1_000;
            const int upperBoundOfRangeOfNumbers = 1_000;
            var countNumbersRandomPositionScores = new int[upperBoundOfRangeOfNumbers].ToList();
            
            for (var i = 0; i < upperIterationBound; i++)
            {
                // act
                var numbers1To1_000 = Enumerable.Range(1, upperBoundOfRangeOfNumbers).ToList().FisherYatesShuffle();
                
                for (var j = 0; j < upperBoundOfRangeOfNumbers; j++)
                {
                    countNumbersRandomPositionScores[j] += numbers1To1_000[j];
                }
            }
            
            // maths bit
            const long average = ((long)upperBoundOfRangeOfNumbers/2) * (upperIterationBound) - ((upperBoundOfRangeOfNumbers/ upperIterationBound) / 2);
            var deviationSquared = countNumbersRandomPositionScores.Sum(x => Math.Pow(x - average, 2));
            var variance = deviationSquared / countNumbersRandomPositionScores.Count;
            var standardDeviation = Math.Sqrt(variance);

            var percentageOfValuesWithin1StdDeviation = (double)(countNumbersRandomPositionScores.Count(x => x > average - standardDeviation && x < average + standardDeviation)) / upperBoundOfRangeOfNumbers * 100;
            var percentageOfValuesWithin2StdDeviation = (double)(countNumbersRandomPositionScores.Count(x => x > average - standardDeviation * 2 && x < average + standardDeviation * 2)) / upperBoundOfRangeOfNumbers * 100;
            var percentageOfValuesWithin3StdDeviation = (double)(countNumbersRandomPositionScores.Count(x => x > average - standardDeviation * 3 && x < average + standardDeviation * 3)) / upperBoundOfRangeOfNumbers * 100;

            // assert
            // With a normally distributed set of numbers you expect to see 68% within one standard deviation of the mean
            // Random placement of elements within our element should result in our countNumbersRandomPositionScores values showing normal distribution
            Assert.That(percentageOfValuesWithin1StdDeviation, Is.EqualTo(68.27).Within(3));
            Assert.That(percentageOfValuesWithin2StdDeviation, Is.EqualTo(95.45).Within(1.5));
            Assert.That(percentageOfValuesWithin3StdDeviation, Is.EqualTo(99.7).Within(0.5));
            
            // generate plot to see normal distribution
            const int bin_width = 2_000;
            
            var binLowerBounds = new List<double>();
            for (var i = average - average * 0.05; i < average + average * 0.05; i += bin_width)
            {
                binLowerBounds.Add(i);
            }
            
            var binCounts = new List<double>();
            for (var i = average - average * 0.05; i < average + average * 0.05; i += bin_width)
            {
                binCounts.Add(countNumbersRandomPositionScores.Count(x => x >= i && x < i + bin_width));
            }
            
            var plt = new Plot(1200, 800);
            
            var bar = plt.AddBar(binCounts.ToArray(), binLowerBounds.ToArray());

            // customize the width of bars (80% of the inter-position distance looks good)
            bar.BarWidth = (binLowerBounds[1] - binLowerBounds[0]);

            // adjust axis limits so there is no padding below the bar graph
            plt.YAxis.Label("Frequency");
            plt.XAxis.Label($"Cumulative sum from {upperIterationBound} iterations of an array of discrete values 1-{upperBoundOfRangeOfNumbers} being shuffled with fisher-yates");
            plt.SetAxisLimits(yMin: 0);
            
            plt.SaveFig("bar_positions.png");
        }

        [Test]
        public void GenerateAllTableCardsFromRequest()
        {
            // arrange
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle();
            
            var request = TestUtilities.GenerateRequest(allCards, Random);
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
            var request = TestUtilities.GenerateRequest(allCards, Random);
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
            var request = TestUtilities.GenerateRequest(allCards, Random);
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
            var request = TestUtilities.GenerateRequest(allCards, Random);
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
            // arrange
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle();
            var request = TestUtilities.GenerateRequest(allCards, Random);
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
            // arrange
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle();
            var request = TestUtilities.GenerateRequest(allCards, Random);
            request.Players[0].Cards = new List<Card>();
            var sharedTableCards = Utilities.GenerateTableCards(request, allCards);
            
            var cardsToBeAppliedToPlayer = allCards.TakeLast(2);
            
            var preGeneratingPlayerAllCardsCount = allCards.Count;

            // act
            var generatedPlayer = Utilities.GeneratePlayer(sharedTableCards, allCards, request.Players[0]);
            
            // assert
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
            // arrange
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle();
            var request = TestUtilities.GenerateRequest(allCards, Random);
            var sharedTableCards = Utilities.GenerateTableCards(request, allCards);

            // act
            var generatedPlayers = Utilities.GeneratePlayers(sharedTableCards, allCards, request);

            // assert
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