using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

#nullable enable
namespace PokerMonteCarloAPI.Tests
{
    [TestFixture]
    public class MonteTests
    {
        private Monte Monte = new Monte();

        [SetUp]
        public void Setup()
        {
            Monte = new Monte();
        }
        
        
        [Test]
        public void TestPerformanceOfMonteCarloMethod()
        {
            var stopwatch = new Stopwatch();
            var allCards = TestUtilities.GenerateRequest(Utilities.GenerateAllCards().ToList(), new Random());
            
            stopwatch.Start();
            Monte.Carlo(allCards);
            stopwatch.Stop();

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            Console.WriteLine(elapsedMilliseconds);
        }
    }
}