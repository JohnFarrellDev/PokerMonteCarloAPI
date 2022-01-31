using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

#nullable enable
namespace PokerMonteCarloAPI.Tests
{
    [TestFixture]
    public class ControllerTests
    {
        private readonly TestUtilities _testUtilities = new TestUtilities();
        private Controller _controller = null!;
        private Mock<IMonte> _mockMonte = null!;
        private List<Card> allCards = null!;
        
        [SetUp]
        public void Setup()
        {
            _mockMonte = new Mock<IMonte>();
            _controller = new Controller(_mockMonte.Object);
            allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle();
        }

        [Test]
        public void WhenMonteReturnsAResponseTheControllerReturnsTheResponseObjectWithA200()
        {
            var request = _testUtilities.GenerateRequest(allCards);
            var expectedResponse = TestUtilities.GenerateResponse();
            _mockMonte.Setup(x => x.Carlo(request, 10_000)).Returns(expectedResponse);

            var response = _controller.MonteCarlo(request) as ObjectResult;

            response?.Value.Should().BeEquivalentTo(expectedResponse);
            response?.StatusCode.Should().Be(200);
        }
        
        [Test]
        public void WhenMonteCarloIsCalledTheMonteServiceIsCalledWithTheSameRequestObject()
        {
            var request = _testUtilities.GenerateRequest(allCards);
            _mockMonte.Setup(x => x.Carlo(request, 10_000));

            _controller.MonteCarlo(request);

            _mockMonte.Verify(x => x.Carlo(request, 10_000));
        }
    }
}