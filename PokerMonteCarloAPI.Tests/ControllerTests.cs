using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PokerMonteCarloAPI.Services;

namespace PokerMonteCarloAPI.Tests
{
    [TestFixture]
    public class ControllerTests
    {
        private Controller _controller = null!;
        private Mock<IMonte> _mockMonte = null!;
        private List<Card> allCards = null!;
        private Mock<IRandomService> mockRandomService = null!;
        
        [SetUp]
        public void Setup()
        {
            _mockMonte = new Mock<IMonte>();
            _controller = new Controller(_mockMonte.Object);
            mockRandomService = new Mock<IRandomService>();
            allCards = Utilities.GenerateAllCards().ToList().FisherYatesShuffle(mockRandomService.Object);
        }

        [Test]
        public void WhenMonteReturnsAResponseTheControllerReturnsTheResponseObjectWithA200()
        {
            var request = TestUtilities.GenerateRequest(allCards, mockRandomService.Object);
            var expectedResponse = TestUtilities.GenerateListPlayerResult(request.Players.Count);
            _mockMonte.Setup(x => x.Carlo(request)).Returns(expectedResponse);

            var response = _controller.MonteCarlo(request) as ObjectResult;

            response?.Value.Should().BeEquivalentTo(expectedResponse);
            response?.StatusCode.Should().Be(200);
        }
        
        [Test]
        public void WhenMonteCarloIsCalledTheMonteServiceIsCalledWithTheSameRequestObject()
        {
            var request = TestUtilities.GenerateRequest(allCards, mockRandomService.Object);
            _mockMonte.Setup(x => x.Carlo(request));

            _controller.MonteCarlo(request);

            _mockMonte.Verify(x => x.Carlo(request));
        }
    }
}