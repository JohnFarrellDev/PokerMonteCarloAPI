using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokerMonteCarloAPI.Services;

namespace PokerMonteCarloAPI
{
    public class Monte : IMonte
    {
        private readonly IRandomService _randomService;

        public Monte(IRandomService randomService)
        {
            _randomService = randomService;
        }

        public List<PlayerResult> Carlo(Request request)
        {

            const int numberOfSimulations = 100_000;
            var allCards = Utilities.GenerateAllCards().ToList();
            var remainingCards = RemovePlayerAndTableCards(allCards, request);

            var lockObject = new object();
            var winTracker = new List<PlayerResult>();

            for (var i = 0; i < request.Players.Count; i++)
            {
                winTracker.Add(new PlayerResult());
            }
            
            var numberOfTasks = Environment.ProcessorCount;
            var simulationsPerThread = numberOfSimulations / numberOfTasks;
            
            var tasks = new Task[numberOfTasks];
            for (var i = 0; i < numberOfTasks; i++)
            {
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    for (var j = 0; j < simulationsPerThread; j++)
                    {
                        var shuffledRemainingCards = remainingCards.ToList().FisherYatesShuffle(_randomService);
                        var tableCards = Utilities.GenerateTableCards(request, shuffledRemainingCards);
                        var players = Utilities.GeneratePlayers(tableCards, shuffledRemainingCards, request);
                        var playersBestHands = players.Select(player => player.CalculateBestHand()).ToList();

                        var winningIndexes = new List<byte> { 0 };

                        for (byte k = 1; k < playersBestHands.Count; k++)
                        {
                            var currentWinner = playersBestHands[winningIndexes[0]];
                            var currentPlayer = playersBestHands[k];

                            if (currentPlayer.Item1 < currentWinner.Item1) continue;
                            if (currentPlayer.Item1 > currentWinner.Item1)
                            {
                                winningIndexes.Clear();
                                winningIndexes.Add(k);
                                continue;
                            }

                            for (var l = 0; l < currentPlayer.Item2.Count; l++)
                            {
                                if (currentPlayer.Item2[l] < currentWinner.Item2[l]) break;

                                if (currentPlayer.Item2[l] > currentWinner.Item2[l])
                                {
                                    winningIndexes.Clear();
                                    winningIndexes.Add(k);
                                    break;
                                }

                                if (l == currentPlayer.Item2.Count - 1)
                                {
                                    winningIndexes.Add(k);
                                }
                            }
                        }

                        lock (lockObject)
                        {
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

                    }
                });
            }

            Task.WaitAll(tasks);
            
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