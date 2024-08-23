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
                new Card(14, 1),
                new Card(13, 1),
                new Card(12, 1),
                new Card(11, 1),
                new Card(10, 1)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(10);
        result.Item2.Should().Equal(new List<byte>() {14,13,12,11,10});
    }

    [Test]
    public void TestStraightFlush()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(13, 1),
                new Card(12, 1),
                new Card(11, 1),
                new Card(10, 1),
                new Card(9, 1)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(9);
        result.Item2.Should().Equal(new List<byte>() {13,12,11,10,9});
    }

    [Test]
    public void TestFourOfAKind()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(14, 1),
                new Card(14, 2),
                new Card(14, 3),
                new Card(14, 4),
                new Card(13, 2)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(8);
        result.Item2.Should().Equal(new List<byte>() {14,14,14,14,13});
    }

    [Test]
    public void TestFullHouse()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(14, 1),
                new Card(14, 2),
                new Card(14, 3),
                new Card(13, 1),
                new Card(13, 2)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(7);
        result.Item2.Should().Equal(new List<byte>() {14,14,14,13,13});
    }

    [Test]
    public void TestFlush()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(14, 1),
                new Card(10, 1),
                new Card(8, 1),
                new Card(6, 1),
                new Card(4, 1)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(6);
        result.Item2.Should().Equal(new List<byte>() {14,10,8,6,4});
    }

    [Test]
    public void TestStraight()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(14, 1),
                new Card(13, 2),
                new Card(12, 3),
                new Card(11, 4),
                new Card(10, 1)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(5);
        result.Item2.Should().Equal(new List<byte>() {14,13,12,11,10});
    }

    [Test]
    public void TestLowStraight()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(5, 1),
                new Card(4, 2),
                new Card(3, 3),
                new Card(2, 4),
                new Card(14, 1)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(5); // Straight
        result.Item2.Should().Equal(new List<byte>() {5,4,3,2,14});
    }

    [Test]
    public void TestFlushAndStraight()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(14, 1),
                new Card(13, 1),
                new Card(12, 2),
                new Card(11, 1),
                new Card(10, 1),
                new Card(9, 1),
                new Card(8, 2)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(6);
        result.Item2.Should().Equal(new List<byte>() {14,13,11,10,9});
    }

    [Test]
    public void TestThreeOfAKind()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(14, 1),
                new Card(14, 2),
                new Card(14, 3),
                new Card(12, 1),
                new Card(10, 2)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(4);
        result.Item2.Should().Equal(new List<byte>() {14,14,14,12,10});
    }

    [Test]
    public void TestTwoPair()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(14, 1),
                new Card(14, 2),
                new Card(13, 1),
                new Card(13, 2),
                new Card(10, 3)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(3);
        result.Item2.Should().Equal(new List<byte>() {14,14,13,13,10});
    }

    [Test]
    public void TestOnePair()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(14, 1),
                new Card(14, 2),
                new Card(13, 1),
                new Card(12, 2),
                new Card(10, 3)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(2);
        result.Item2.Should().Equal(new List<byte>() {14,14,13,12,10});
    }

    [Test]
    public void TestHighCard()
    {
        var player = new Player
        (
            new List<Card>()
            {
                new Card(14, 1),
                new Card(13, 2),
                new Card(11, 3),
                new Card(9, 4),
                new Card(7, 1)
            }, false
        );

        var result = player.CalculateBestHand();

        result.Item1.Should().Be(1);
        result.Item2.Should().Equal(new List<byte>() {14,13,11,9,7});
    }
}