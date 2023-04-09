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
        private readonly Random _random = new Random();
        
        [Test]
        public void GenerateAllCardsProduced52DistinctPlayingCards()
        {
            var allCards = Utilities.GenerateAllCards().ToList();
            var allCardsSet = new HashSet<Card>(allCards);

            allCards.Count.Should().Be(52);
            allCardsSet.Count.Should().Be(52);

            allCardsSet.Count(card => card.Suit == Suit.Clubs).Should().Be(13);
            allCardsSet.Count(card => card.Suit == Suit.Diamonds).Should().Be(13);
            allCardsSet.Count(card => card.Suit == Suit.Hearts).Should().Be(13);
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
            new object[] {1, new List<int> {4,3,5,1,2}},
            new object[] {11, new List<int> {4,5,2,1,3}},
            new object[] {111, new List<int> {3,1,5,2,4}},
            new object[] {1111, new List<int> {5,3,2,4,1}},
            new object[] {11111, new List<int> {5,3,1,4,2}},
            new object[] {2, new List<int> {3,5,1,2,4}},
            new object[] {22, new List<int> {4,3,1,5,2}},
            new object[] {222, new List<int> {1,2,3,5,4}},
            new object[] {2222, new List<int> {1,2,5,4,3}},
            new object[] {22222, new List<int> {4,2,3,5,1}},
            new object[] {222222, new List<int> {2,3,5,4,1}},
            new object[] {3, new List<int> {5,1,4,3,2}},
            new object[] {33, new List<int> {1,2,4,3,5}},
            new object[] {333, new List<int> {1,5,2,3,4}},
            new object[] {3333, new List<int> {1,3,2,4,5}},
            new object[] {33333, new List<int> {5,3,1,2,4}},
            new object[] {333333, new List<int> {3,1,5,4,2}}
        };
        [TestCaseSource(nameof(_seededFisherYatesShuffle))]
        public void FisherYatesWhenRandomSeededProducesSameOutput(int seed, List<int> expectedOutput)
        {
            var shuffledNumbers1To5 = Enumerable.Range(1, 5).ToList().FisherYatesShuffle(seed);

            shuffledNumbers1To5.Should().ContainInOrder(expectedOutput);
        }
        
        [Test]
        public void FisherYatesShouldRandomlyShuffleAList()
        {
            // arrange
            const int upperIterationBound = 5_000;
            const int upperBoundOfRangeOfNumbers = 5_000;
            var countNumbersRandomPositionScores = new int[upperBoundOfRangeOfNumbers].ToList();
            
            for (var i = 0; i < upperIterationBound; i++)
            {
                // act
                var numbers1To1000 = Enumerable.Range(1, upperBoundOfRangeOfNumbers).ToList().FisherYatesShuffle();
                
                for (var j = 0; j < upperBoundOfRangeOfNumbers; j++)
                {
                    countNumbersRandomPositionScores[j] += numbers1To1000[j];
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
            const int binWidth = 2_000;
            
            var binLowerBounds = new List<double>();
            for (var i = average - average * 0.05; i < average + average * 0.05; i += binWidth)
            {
                binLowerBounds.Add(i);
            }
            
            var binCounts = new List<double>();
            for (var i = average - average * 0.05; i < average + average * 0.05; i += binWidth)
            {
                binCounts.Add(countNumbersRandomPositionScores.Count(x => x >= i && x < i + binWidth));
            }
            
            var plt = new Plot(1200, 800);
            
            var bar = plt.AddBar(binCounts.ToArray(), binLowerBounds.ToArray());

            // customize the width of bars (80% of the inter-position distance looks good)
            bar.BarWidth = (binLowerBounds[1] - binLowerBounds[0]);

            // adjust axis limits so there is no padding below the bar graph
            plt.YAxis.Label("Frequency");
            plt.XAxis.Label($"Cumulative sum from {upperIterationBound} iterations of an array of discrete values 1-{upperBoundOfRangeOfNumbers} being shuffled with fisher-yates");
            plt.SetAxisLimits(yMin: 0);
            
            plt.Style(Style.Gray1);
            var bnColor = System.Drawing.ColorTranslator.FromHtml("#2e3440");
            plt.Style(bnColor, bnColor);
            
            plt.SaveFig("bar_positions.png");
        }

        [Test]
        public void GenerateAllTableCardsFromRequest()
        {
            // arrange
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle();
            
            var request = TestUtilities.GenerateRequest(allCards, _random);
            while (request.TableCards.Count < 5)
            {
                request.TableCards.Add(allCards.Pop());
            }

            // act
            var tableCards = Utilities.GenerateTableCards(request, allCards);

            // assert
            tableCards.Count.Should().Be(5);
            tableCards.Should().ContainInOrder(request.TableCards);
        }

        [Test]
        public void GenerateAllTableCardsFromDeck()
        {
            // arrange
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle();
            var request = TestUtilities.GenerateRequest(allCards, _random);
            request.TableCards = new List<Card>();
            var top5CardsFromDeck = allCards.TakeLast(5).ToList();
            top5CardsFromDeck.Reverse();
            
            // act
            var tableCards = Utilities.GenerateTableCards(request, allCards);

            // assert
            tableCards.Count.Should().Be(5);
            tableCards.Should().ContainInOrder(top5CardsFromDeck);
        }

        [Test]
        public void GenerateTableCardsFromRequestAndDeck()
        {
            // arrange
            const int numberOfCardsFromRequests = 3;
            const int numberOfCardsFromDeck = 2;
            
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle();
            var request = TestUtilities.GenerateRequest(allCards, _random);
            request.TableCards = new List<Card>();
            
            while (request.TableCards.Count < numberOfCardsFromRequests)
            {
                request.TableCards.Add(allCards.Pop());
            }

            var cardsFromDeck = allCards.TakeLast(numberOfCardsFromDeck).ToList();
            cardsFromDeck.Reverse();
            
            // act
            var tableCards = Utilities.GenerateTableCards(request, allCards);
            
            // assert
            tableCards.Count.Should().Be(5);
            tableCards.Should().ContainInOrder(request.TableCards.Concat(cardsFromDeck));
        }

        [Test]
        public void TestGeneratePlayerWithFullHandAndTableCards()
        {
            // arrange
            var allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle();
            var request = TestUtilities.GenerateRequest(allCards, _random);
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
            var request = TestUtilities.GenerateRequest(allCards, _random);
            request.TableCards = new List<Card>();
            var deckCardsToBeAdded = allCards.TakeLast(5).ToList();

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
            var request = TestUtilities.GenerateRequest(allCards, _random);
            request.Players[0].Cards = new List<Card>();
            var sharedTableCards = Utilities.GenerateTableCards(request, allCards);
            
            var cardsToBeAppliedToPlayer = allCards.TakeLast(2).ToArray();
            
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
            var request = TestUtilities.GenerateRequest(allCards, _random);
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

        // private static object[] playerHandsToCalculateBestHand =
        // {
        //     new object[] {333333, new List<int> {3,1,5,4,2}}
        // };
        // [TestCaseSource(nameof(playerHandsToCalculateBestHand))]
        // public void CalculateBestHandReturnsExpectedHandAndHighCards(int v1, List<int> v2)
        // {
        //     true.Should().BeTrue();
        // }
    }
}