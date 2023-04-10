using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerMonteCarloAPI
{
    public class Monte : IMonte
    {
        public List<PlayerResult> Carlo(Request request)
        {
            const int numberOfSimulations = 1_000_000;
            var allCards = Utilities.GenerateAllCards().ToList();
            var remainingCards = RemovePlayerAndTableCards(allCards, request);
            
            // calculate each players best hand
            // first best hand but also 5 highest cards that make the hand
            // determine winner
            // keep score for how often each player has won
            // extra - keep track of each players hand over time, return what percentage of time we got certain hands
            
            var winTracker = new List<PlayerResult>();
            for (var i = 0; i < request.Players.Count; i++)
            {
                winTracker.Add(new PlayerResult());
            }
            
            for (var i = 0; i < numberOfSimulations; i++)
            {
                var shuffledRemainingCards = remainingCards.ToList().FisherYatesShuffle();
                var tableCards = Utilities.GenerateTableCards(request, shuffledRemainingCards);
                var players = Utilities.GeneratePlayers(tableCards, shuffledRemainingCards, request);
                var playersBestHands = players.Select(player => player.CalculateBestHand()).ToList();
                
                var winningIndexes = new List<byte>{0};    
                
                for (byte j = 1; j < playersBestHands.Count; j++)
                {
                    var currentWinner = playersBestHands[winningIndexes[0]];
                    var currentPlayer = playersBestHands[j];
                    
                    if(currentPlayer.Item1 < currentWinner.Item1) continue;
                    if (currentPlayer.Item1 > currentWinner.Item1)
                    {
                        winningIndexes.Clear();
                        winningIndexes.Add(j);
                        continue;
                    }
                    for (var k = 0; k < currentPlayer.Item2.Count; k++)
                    {
                        if(currentPlayer.Item2[k] < currentWinner.Item2[k]) break;
                        
                        if (currentPlayer.Item2[k] > currentWinner.Item2[k])
                        {
                            winningIndexes.Clear();
                            winningIndexes.Add(j);
                            break;
                        }
                        
                        if (k == currentPlayer.Item2.Count - 1)
                        {
                            winningIndexes.Add(j);
                        }
                    }
                }
                
                switch (winningIndexes.Count)
                    {
                        case 1:
                            winTracker[winningIndexes[0]].WinCount++;
                            break;
                        case 2:
                            winTracker[winningIndexes[0]].TwoWayTieCount++;
                            winTracker[winningIndexes[1]].TwoWayTieCount++;
                            break;
                        case 3:
                            winTracker[winningIndexes[0]].ThreeWayTieCount++;
                            winTracker[winningIndexes[1]].ThreeWayTieCount++;
                            winTracker[winningIndexes[2]].ThreeWayTieCount++;
                            break;
                        case 4:
                            winTracker[winningIndexes[0]].FourWayTieCount++;
                            winTracker[winningIndexes[1]].FourWayTieCount++;
                            winTracker[winningIndexes[2]].FourWayTieCount++;
                            winTracker[winningIndexes[3]].FourWayTieCount++;
                            break;
                        case 5:
                            winTracker[winningIndexes[0]].FiveWayTieCount++;
                            winTracker[winningIndexes[1]].FiveWayTieCount++;
                            winTracker[winningIndexes[2]].FiveWayTieCount++;
                            winTracker[winningIndexes[3]].FiveWayTieCount++;
                            winTracker[winningIndexes[4]].FiveWayTieCount++;
                            break;
                        case 6:
                            winTracker[winningIndexes[0]].SixWayTieCount++;
                            winTracker[winningIndexes[1]].SixWayTieCount++;
                            winTracker[winningIndexes[2]].SixWayTieCount++;
                            winTracker[winningIndexes[3]].SixWayTieCount++;
                            winTracker[winningIndexes[4]].SixWayTieCount++;
                            winTracker[winningIndexes[5]].SixWayTieCount++;
                            break;
                        case 7:
                            winTracker[winningIndexes[0]].SevenWayTieCount++;
                            winTracker[winningIndexes[1]].SevenWayTieCount++;
                            winTracker[winningIndexes[2]].SevenWayTieCount++;
                            winTracker[winningIndexes[3]].SevenWayTieCount++;
                            winTracker[winningIndexes[4]].SevenWayTieCount++;
                            winTracker[winningIndexes[5]].SevenWayTieCount++;
                            winTracker[winningIndexes[6]].SevenWayTieCount++;
                            break;
                        case 8:
                            winTracker[winningIndexes[0]].EightWayTieCount++;
                            winTracker[winningIndexes[1]].EightWayTieCount++;
                            winTracker[winningIndexes[2]].EightWayTieCount++;
                            winTracker[winningIndexes[3]].EightWayTieCount++;
                            winTracker[winningIndexes[4]].EightWayTieCount++;
                            winTracker[winningIndexes[5]].EightWayTieCount++;
                            winTracker[winningIndexes[6]].EightWayTieCount++;
                            winTracker[winningIndexes[7]].EightWayTieCount++;
                            break;
                        case 9:
                            winTracker[winningIndexes[0]].NineWayTieCount++;
                            winTracker[winningIndexes[1]].NineWayTieCount++;
                            winTracker[winningIndexes[2]].NineWayTieCount++;
                            winTracker[winningIndexes[3]].NineWayTieCount++;
                            winTracker[winningIndexes[4]].NineWayTieCount++;
                            winTracker[winningIndexes[5]].NineWayTieCount++;
                            winTracker[winningIndexes[6]].NineWayTieCount++;
                            winTracker[winningIndexes[7]].NineWayTieCount++;
                            winTracker[winningIndexes[8]].NineWayTieCount++;
                            break;
                    }
            }
            
            return winTracker;
        }

        private List<Card> RemovePlayerAndTableCards(IEnumerable<Card> allCards, Request request)
        {
            var setOfAllCards = new HashSet<Card>(allCards);

            foreach (var card in request.TableCards)
            {
                setOfAllCards.Remove(card);
            }
            
            foreach (var card in request.Players.SelectMany(player => player.Cards))
            {
                setOfAllCards.Remove(card);
            }

            return setOfAllCards.ToList();
        }
    }
}