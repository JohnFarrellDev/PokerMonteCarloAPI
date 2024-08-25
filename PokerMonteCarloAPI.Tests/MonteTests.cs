using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using PokerMonteCarloAPI.Services;
using Moq;

#nullable enable
namespace PokerMonteCarloAPI.Tests
{
    [TestFixture]
    public class MonteTests
    {
        private Monte Monte = null!;
        private Mock<IRandomService> mockRandomService = null!;

        [SetUp]
        public void Setup()
        {
            mockRandomService = new Mock<IRandomService>();
            Monte = new Monte(mockRandomService.Object);
        }
        
        
        [Test]
        public void TestPerformanceOfMonteCarloMethod()
        {
            var stopwatch = new Stopwatch();
            var allCards = TestUtilities.GenerateRequest(Utilities.GenerateAllCards().ToList(), mockRandomService.Object);
            
            stopwatch.Start();
            Monte.Carlo(allCards);
            stopwatch.Stop();

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            Console.WriteLine(elapsedMilliseconds);
        }
    }
}