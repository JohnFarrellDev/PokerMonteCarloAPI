using System;
using System.Collections.Generic;
using System.Linq;
using PokerMonteCarloAPI.Services;

#nullable enable
namespace PokerMonteCarloAPI.Tests
{
    public static class TestUtilities
    {
        public static Request GenerateRequest(List<Card> allCards, IRandomService randomService, int? numberOfPlayers = null)
        {
            return new Request
            {
                TableCards = GenerateTableCards(allCards, randomService).ToList(),
                Players = GenerateTestPlayers(allCards, randomService, numberOfPlayers).ToList()
            };
        }

        public static List<PlayerResult> GenerateListPlayerResult(int numberOfPlayers)
        {
            var playerResults = new List<PlayerResult>();
            for (var i = 0; i < numberOfPlayers; i++)
            {
                playerResults.Add(new PlayerResult
                {
                    WinCount = 0,
                    LoseCount = 0,
                    TwoWayTieCount = 0,
                    ThreeWayTieCount = 0,
                    FourWayTieCount = 0,
                    FiveWayTieCount = 0,
                    SixWayTieCount = 0,
                    SevenWayTieCount = 0,
                    EightWayTieCount = 0,
                    NineWayTieCount = 0
                });
            }

            return playerResults;
        }


        public static IEnumerable<Card> GenerateTableCards(List<Card> allCards, IRandomService randomService)
        {
            for(var i = 0; i < randomService.Next(6); i++)
            {
                yield return allCards.Pop();
            }
        }

        public static IEnumerable<PlayerRequest> GenerateTestPlayers(List<Card> allCards, IRandomService randomService, int? numberOfPlayers = null, int numberOfPlayersFolded = 0)
        {
            numberOfPlayers ??= randomService.Next(13) + 2;

            
            for (var i = 0; i < numberOfPlayers; i++)
            {
                yield return GeneratePlayerRequest(allCards, 2, numberOfPlayersFolded-- > 0);
            }
        }
        
        private static PlayerRequest GeneratePlayerRequest(List<Card> allCards, int numberOfCards, bool folded)
        {
            return new PlayerRequest
            {
                Cards = GeneratePlayerCards(allCards, numberOfCards).ToList(),
                Folded = folded
            };
        }

        private static IEnumerable<Card> GeneratePlayerCards(List<Card> allCards, int numberOfCards)
        {
            for (var i = 0; i < numberOfCards; i++)
            {
                yield return allCards.Pop();
            }
        }
    }
}