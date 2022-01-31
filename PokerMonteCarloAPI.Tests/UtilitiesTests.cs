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
        
        // test fisher yates shuffle algorithm
    }
}