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
        [Repeat(10)]
        public void FisherYatesShouldRandomlyShuffleOurListOf52PlayingCards()
        {
            var countCardShuffledPosition = new Dictionary<Card, int>();
            
            for (var i = 0; i < 100; i++)
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

            // standard deviation of 0 means complete randomness
            // standard deviation of 1500 when fisher-yates not applied
            // standard deviation typically between 130-150 with fisher-yates
            standardDeviation.Should().BeLessThan(200);
        }
    }
}