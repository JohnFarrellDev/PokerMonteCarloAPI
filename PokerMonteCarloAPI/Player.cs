using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace PokerMonteCarloAPI
{
    public class Player
    {
        public List<Card> PlayersHand { get; }
        private readonly bool _folded;
        private readonly Dictionary<Value, int> _valueCounts;
        private readonly Dictionary<Suit, int> _suitCounts;
        private readonly List<Value> _sortedDescending;

        public Player(List<Card> playersHand, bool folded)
        {
            PlayersHand = playersHand;
            _folded = folded;
            _valueCounts = new Dictionary<Value, int>();
            _suitCounts = new Dictionary<Suit, int>();
            foreach (var card in playersHand)
            {
                if (!_valueCounts.ContainsKey(card.Value))
                {
                    _valueCounts[card.Value] = 0;
                }
                _valueCounts[card.Value]++;
                if (!_suitCounts.ContainsKey(card.Suit))
                {
                    _suitCounts[card.Suit] = 0;
                }
                _suitCounts[card.Suit]++;
            }
            _sortedDescending = playersHand.Select(c => c.Value).OrderByDescending(v => v).ToList();
        }

        public (byte, List<Value>) CalculateBestHand()
        {
            if(_folded) return (0, new List<Value>());
            
            var (hasFlush, anyFlushHighCards, flushType) = HasAnyFlush();
            
            if (flushType == 10) return (10, anyFlushHighCards!);
            if (flushType == 9) return (9, anyFlushHighCards!);
            
            var (hasFourOfAKind, fourOfAKindHighCards) = HasFourOfAKind();
            if(hasFourOfAKind) return (8, fourOfAKindHighCards!);
            
            var (hasFullHouse, fullHouseHighCards) = HasFullHouse();
            if(hasFullHouse) return (7, fullHouseHighCards!);
            
            if (hasFlush) return (6, anyFlushHighCards!);
            
            var (hasStraight, straightCards) = HasStraight();
            if (hasStraight) return (5, straightCards!);
            
            var (hasThreeOfAKind, threeOfAKindHighCards) = HasThreeOfAKind();
            if(hasThreeOfAKind) return (4, threeOfAKindHighCards!);
            
            var (hasTwoPair, twoPairHighCards) = HasTwoPair();
            if(hasTwoPair) return (3, twoPairHighCards!);
            
            var (hasPair, pairHighCards) = HasPair();
            if(hasPair) return (2, pairHighCards!);
            
            return (1, _sortedDescending.Take(5).ToList());
        }

        private static (bool, List<Value>?) HasRoyalFlush(List<Card> flushCards)
        {
            var royalFlush = new List<Value> { Value.Ten, Value.Jack, Value.Queen, Value.King, Value.Ace };
            var isRoyalFlush = royalFlush.All(v => flushCards.Select(c => c.Value).Contains(v));

            if (isRoyalFlush)
            {
                return (true, new List<Value>
                {
                    Value.Ace,
                    Value.King,
                    Value.Queen,
                    Value.Jack,
                    Value.Ten,
                });
            }

            return (false, null);
        }

        public (bool, List<Value>?) HasStraightFlush(List<Card> flushCards)
        {
            var sortedValues = flushCards.Select(c => c.Value).Distinct().OrderByDescending(v => v).ToList();
            if(sortedValues[0] == Value.Ace) sortedValues.Add(Value.LowAce);
            
            for (var i = 0; i < sortedValues.Count - 4; i++)
            {
                var currentVal = (int)sortedValues[i];
                if ((int)sortedValues[i + 1] != currentVal - 1 || (int)sortedValues[i + 2] != currentVal - 2
                    || (int)sortedValues[i + 3] != currentVal - 3 || (int)sortedValues[i + 4] != currentVal - 4) continue;
                return (true, new List<Value>
                {
                    sortedValues[i], sortedValues[i + 1], sortedValues[i + 2], sortedValues[i + 3],
                    sortedValues[i + 4]
                });
            }
            
            return (false, null);
        }
        
        public (bool, List<Value>?) HasFourOfAKind()
        {
            foreach (var count in _valueCounts)
            {
                if(count.Value != 4) continue;

                var fourOfAKindRank = _valueCounts.FirstOrDefault(x => x.Value == 4).Key;
                var kicker = PlayersHand.Select(card => card.Value).Where(value => value != fourOfAKindRank).MaxBy(v => v);

                return (true, new List<Value>
                    { fourOfAKindRank, fourOfAKindRank, fourOfAKindRank, fourOfAKindRank, kicker });
            }

            return (false, null);
        }

        public (bool, List<Value>?) HasFullHouse()
        {
            var threeOfAKindValues = new List<Value>();
            var pairValues = new List<Value>();
            
            foreach (var valueCount in _valueCounts)
            {
                switch (valueCount.Value)
                {
                    case 3:
                        threeOfAKindValues.Add(valueCount.Key);
                        break;
                    case 2:
                        pairValues.Add(valueCount.Key);
                        break;
                }
            }

            switch (threeOfAKindValues.Count)
            {
                case 0:
                    return (false, null);
                case 1 when pairValues.Count == 0:
                    return (false, null);
                case 1 when pairValues.Count == 1:
                {
                    var threeOfAKindValue = threeOfAKindValues[0];
                    var pairValue = pairValues[0];
                    return (true, new List<Value> { threeOfAKindValue, threeOfAKindValue, threeOfAKindValue, pairValue, pairValue });
                }
                case 1 when pairValues.Count == 2:
                    var pairValue1 = pairValues[0];
                    var pairValue2 = pairValues[1];
                    if (pairValue1 > pairValue2)
                    {
                        return (true, new List<Value> { threeOfAKindValues[0], threeOfAKindValues[0], threeOfAKindValues[0], pairValue1, pairValue1 });
                    }
                    return (true, new List<Value> { threeOfAKindValues[0], threeOfAKindValues[0], threeOfAKindValues[0], pairValue2, pairValue2 });
                case 2:
                {
                    var threeOfAKindValue1 = threeOfAKindValues[0];
                    var threeOfAKindValue2 = threeOfAKindValues[1];
                    if (threeOfAKindValue1 > threeOfAKindValue2)
                    {
                        return (true, new List<Value> { threeOfAKindValue1, threeOfAKindValue1, threeOfAKindValue1, threeOfAKindValue2, threeOfAKindValue2 });
                    }
                    return (true, new List<Value> { threeOfAKindValue2, threeOfAKindValue2, threeOfAKindValue2, threeOfAKindValue1, threeOfAKindValue1 });
                }
                default:
                    return (false, null);        
            }
        }
        
        public (bool, List<Value>?, byte?) HasAnyFlush()
        {
            Suit? flushSuit = null;
            foreach (var count in _suitCounts)
            {
                if (count.Value < 5) continue;
                flushSuit = count.Key;
                break;
            }
            
            if (flushSuit == null)
            {
                return (false, null, null);
            }
            
            var flushCards = PlayersHand.Where(card => card.Suit == flushSuit).ToList();

            var (hasRoyalFlush, royalFlushCards) = HasRoyalFlush(flushCards);
            if (hasRoyalFlush)
            {
                return (true, royalFlushCards, 10);
            }
            
            var (hasStraightFlush, straightFlushCards) = HasStraightFlush(flushCards);
            if (hasStraightFlush)
            {
                return (true, straightFlushCards, 9);
            }
            
            return (true, flushCards.Select(card => card.Value).OrderByDescending(v => v).Take(5).ToList(), 6);
        }

        public (bool, List<Value>?) HasStraight()
        {
            var sortedValues = _sortedDescending.Distinct().ToList();
            
            if(sortedValues[0] == Value.Ace) sortedValues.Add(Value.LowAce);
            
            for (var i = 0; i < sortedValues.Count - 4; i++)
            {
                var currentVal = (int)sortedValues[i];
                if ((int)sortedValues[i + 1] != currentVal - 1 || (int)sortedValues[i + 2] != currentVal - 2
                    || (int)sortedValues[i + 3] != currentVal - 3 || (int)sortedValues[i + 4] != currentVal - 4) continue;
                return (true, new List<Value>
                {
                    sortedValues[i], sortedValues[i + 1], sortedValues[i + 2], sortedValues[i + 3],
                    sortedValues[i + 4]
                });
            }
            
            return (false, null);
        }
        
        public (bool, List<Value>?) HasThreeOfAKind()
        {
            foreach (var count in _valueCounts)
            {
                if(count.Value != 3) continue;

                var threeOfAKindRank = _valueCounts.FirstOrDefault(x => x.Value == 3).Key;
                var descendingHighCards = _sortedDescending.Where(value => value != threeOfAKindRank).ToList();
                var kicker1 = descendingHighCards[0];
                var kicker2 = descendingHighCards[1];

                return (true, new List<Value>
                    { threeOfAKindRank, threeOfAKindRank, threeOfAKindRank, kicker1, kicker2 });
            }

            return (false, null);
        }

        public (bool, List<Value>?) HasTwoPair()
        {
            var pairValues = new List<Value>();
            foreach (var valueCount in _valueCounts)
            {
                if (valueCount.Value != 2) continue;
                pairValues.Add(valueCount.Key);
            }

            if (pairValues.Count < 2) return (false, null);
            
            var pairValuesSortedDescending = pairValues.OrderByDescending(v => v).ToList();
            
            var kicker = pairValues.Count == 2  ? _sortedDescending
                    .Where(value => value != pairValuesSortedDescending[0] && 
                                    value != pairValuesSortedDescending[1]).MaxBy(v => v)
                :
                _sortedDescending
                    .Where(value => value != pairValuesSortedDescending[0] && 
                                    value != pairValuesSortedDescending[1] && 
                                    value != pairValuesSortedDescending[2]).MaxBy(v => v);
            
            return (true, new List<Value>
                { pairValuesSortedDescending[0], pairValuesSortedDescending[0], pairValuesSortedDescending[1], pairValuesSortedDescending[1], kicker });
        }
        
        public (bool, List<Value>?) HasPair()
        {
            foreach (var count in _valueCounts)
            {
                if (count.Value != 2) continue;
                var pairRank = _valueCounts.First(x => x.Value == 2).Key;
                var descendingHighCards = _sortedDescending.Where(value => value != pairRank).ToList();
                var kicker1 = descendingHighCards[0];
                var kicker2 = descendingHighCards[1];
                var kicker3 = descendingHighCards[2];

                return (true, new List<Value>
                    { pairRank, pairRank, kicker1, kicker2, kicker3 });
            }

            return (false, null);
        }
    }
}