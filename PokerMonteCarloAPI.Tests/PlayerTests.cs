using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace PokerMonteCarloAPI.Tests;

[TestFixture]
public class PlayerTests
{
    [Test]
    public void TestRoyalFlush()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(Value.Ace, Suit.Hearts),
                new Card(Value.King, Suit.Hearts),
                new Card(Value.Queen, Suit.Hearts),
                new Card(Value.Jack, Suit.Hearts),
                new Card(Value.Ten, Suit.Hearts)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(10);
        result.Item2.Should().Equal(new List<Value>() {Value.Ace, Value.King, Value.Queen, Value.Jack, Value.Ten});
    }

    [Test]
    public void TestStraightFlush()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(Value.King, Suit.Hearts),
                new Card(Value.Queen, Suit.Hearts),
                new Card(Value.Jack, Suit.Hearts),
                new Card(Value.Ten, Suit.Hearts),
                new Card(Value.Nine, Suit.Hearts)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(9);
        result.Item2.Should().Equal(new List<Value>() {Value.King, Value.Queen, Value.Jack, Value.Ten, Value.Nine});
    }

    [Test]
    public void TestFourOfAKind()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(Value.Ace, Suit.Hearts),
                new Card(Value.Ace, Suit.Diamonds),
                new Card(Value.Ace, Suit.Clubs),
                new Card(Value.Ace, Suit.Spades),
                new Card(Value.King, Suit.Diamonds)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(8);
        result.Item2.Should().Equal(new List<Value>() {Value.Ace, Value.Ace, Value.Ace, Value.Ace, Value.King});
    }

    [Test]
    public void TestFullHouse()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(Value.Ace, Suit.Hearts),
                new Card(Value.Ace, Suit.Diamonds),
                new Card(Value.Ace, Suit.Clubs),
                new Card(Value.King, Suit.Hearts),
                new Card(Value.King, Suit.Diamonds)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(7);
        result.Item2.Should().Equal(new List<Value>() {Value.Ace, Value.Ace, Value.Ace, Value.King, Value.King});
    }

    [Test]
    public void TestFlush()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(Value.Ace, Suit.Hearts),
                new Card(Value.Ten, Suit.Hearts),
                new Card(Value.Eight, Suit.Hearts),
                new Card(Value.Six, Suit.Hearts),
                new Card(Value.Four, Suit.Hearts)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(6);
        result.Item2.Should().Equal(new List<Value>() {Value.Ace, Value.Ten, Value.Eight, Value.Six, Value.Four});
    }

    [Test]
    public void TestStraight()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(Value.Ace, Suit.Hearts),
                new Card(Value.King, Suit.Diamonds),
                new Card(Value.Queen, Suit.Clubs),
                new Card(Value.Jack, Suit.Spades),
                new Card(Value.Ten, Suit.Hearts)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(5);
        result.Item2.Should().Equal(new List<Value>() {Value.Ace, Value.King, Value.Queen, Value.Jack, Value.Ten});
    }

    [Test]
    public void TestLowStraight()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(Value.Five, Suit.Hearts),
                new Card(Value.Four, Suit.Diamonds),
                new Card(Value.Three, Suit.Clubs),
                new Card(Value.Two, Suit.Spades),
                new Card(Value.Ace, Suit.Hearts)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(5); // Straight
        result.Item2.Should().Equal(new List<Value>() {Value.Five, Value.Four, Value.Three, Value.Two, Value.LowAce});
    }

    [Test]
    public void TestFlushAndStraight()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(Value.Ace, Suit.Hearts),
                new Card(Value.King, Suit.Hearts),
                new Card(Value.Queen, Suit.Diamonds),
                new Card(Value.Jack, Suit.Hearts),
                new Card(Value.Ten, Suit.Hearts),
                new Card(Value.Nine, Suit.Hearts),
                new Card(Value.Eight, Suit.Diamonds)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(6);
        result.Item2.Should().Equal(new List<Value>() {Value.Ace, Value.King, Value.Jack, Value.Ten, Value.Nine});
    }

    [Test]
    public void TestThreeOfAKind()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(Value.Ace, Suit.Hearts),
                new Card(Value.Ace, Suit.Diamonds),
                new Card(Value.Ace, Suit.Clubs),
                new Card(Value.Queen, Suit.Hearts),
                new Card(Value.Ten, Suit.Diamonds)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(4);
        result.Item2.Should().Equal(new List<Value>() {Value.Ace, Value.Ace, Value.Ace, Value.Queen, Value.Ten});
    }

    [Test]
    public void TestTwoPair()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(Value.Ace, Suit.Hearts),
                new Card(Value.Ace, Suit.Diamonds),
                new Card(Value.King, Suit.Hearts),
                new Card(Value.King, Suit.Diamonds),
                new Card(Value.Ten, Suit.Clubs)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(3);
        result.Item2.Should().Equal(new List<Value>() {Value.Ace, Value.Ace, Value.King, Value.King, Value.Ten});
    }

    [Test]
    public void TestOnePair()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(Value.Ace, Suit.Hearts),
                new Card(Value.Ace, Suit.Diamonds),
                new Card(Value.King, Suit.Hearts),
                new Card(Value.Queen, Suit.Diamonds),
                new Card(Value.Ten, Suit.Clubs)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(2);
        result.Item2.Should().Equal(new List<Value>() {Value.Ace, Value.Ace, Value.King, Value.Queen, Value.Ten});
    }

    [Test]
    public void TestHighCard()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(Value.Ace, Suit.Hearts),
                new Card(Value.King, Suit.Diamonds),
                new Card(Value.Jack, Suit.Spades),
                new Card(Value.Nine, Suit.Clubs),
                new Card(Value.Seven, Suit.Hearts)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(1);
        result.Item2.Should().Equal(new List<Value>() {Value.Ace, Value.King, Value.Jack, Value.Nine, Value.Seven});
    }
}