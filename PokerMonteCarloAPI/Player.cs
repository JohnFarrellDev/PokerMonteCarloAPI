using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace PokerMonteCarloAPI
{
    public class Player
    {
        private readonly List<Card> _playersHand;
        private readonly bool _folded;
        private readonly Dictionary<Value, byte> _valueCounts;
        private readonly Dictionary<Suit, byte> _suitCounts;
        private readonly List<Value> _sortedDescending;
        private readonly Dictionary<byte, byte> _sameValueCount;


        public Player(List<Card> playersHand, bool folded)
        {
            _playersHand = playersHand;
            _folded = folded;
            _valueCounts = new Dictionary<Value, byte>();
            _suitCounts = new Dictionary<Suit, byte>();
            foreach (var card in playersHand)
            {
                if (!_valueCounts.ContainsKey(card.value))
                {
                    _valueCounts[card.value] = 0;
                }
                _valueCounts[card.value]++;
                if (!_suitCounts.ContainsKey(card.suit))
                {
                    _suitCounts[card.suit] = 0;
                }
                _suitCounts[card.suit]++;
            }
            _sortedDescending = playersHand.Select(c => c.value).OrderByDescending(v => v).ToList();
            _sameValueCount = new Dictionary<byte, byte>();
        }

        public (Hand, List<Card>) CalculateBestHand()
        {
            // calculate the best poker hand possible from the PLayersHand and the table cards
            // return the best hand and the high cards in order

            var (hasFlush, anyFlushHighCards, flushType) = HasAnyFlush();

            

            // calculate if the List<Card> contains a royal flush (DONE)
            if (flushType == Hand.RoyalFlush) return (Hand.RoyalFlush, anyFlushHighCards!);
            
            // calculate if the List<Card> contains a straight flush (DONE)
            if (flushType == Hand.StraightFlush) return (Hand.StraightFlush, anyFlushHighCards!);
            
            // calculate if the List<Card> contains a four of a kind
            
            
            // calculate if the List<Card> contains a full house
            var (hasFullHouse, fullHouseHighCards) = HasFullHouse();
            if(hasFullHouse) return (Hand.FullHouse, fullHouseHighCards!);
            
            // calculate if the List<Card> contains a flush (DONE)
            if (hasFlush) return (Hand.Flush, anyFlushHighCards!);
            // calculate if the List<Card> contains a straight (DONE)
            // calculate if the List<Card> contains a three of a kind
            // calculate if the List<Card> contains a two pair
            // calculate if the List<Card> contains a pair
            // calculate if the List<Card> contains a high card



        }

        // flushCards will always be belonging to the same Suit
        private static (bool, List<Card>) HasRoyalFlush(List<Card> flushCards)
        {
            var royalFlush = new List<Value> { Value.Ten, Value.Jack, Value.Queen, Value.King, Value.Ace };
            var isRoyalFlush = royalFlush.All(v => flushCards.Select(c => c.value).Contains(v));

            if (isRoyalFlush)
            {
                return (true, new List<Card>
                {
                    flushCards.Find(v => v.value == Value.Ace)!,
                    flushCards.Find(v => v.value == Value.King)!,
                    flushCards.Find(v => v.value == Value.Queen)!,
                    flushCards.Find(v => v.value == Value.Jack)!,
                    flushCards.Find(v => v.value == Value.Ten)!,
                });
            }

            return (false, new List<Card>());
        }

        // flushCards will always be belonging to the same Suit
        public (bool, List<Card>?) HasStraightFlush(List<Card> flushCards)
        {
            var sortedValues = flushCards.Select(c => c.value).Distinct().OrderByDescending(v => v).ToList();
            var straightValues = new List<Value>();
    
            // Check for Ace low straight
            var aceLowStraight = new List<Value>() { Value.Five, Value.Four, Value.Three, Value.Two, Value.Ace };
            if (aceLowStraight.All(sortedValues.Contains))
            {
                straightValues = new List<Value>() { Value.Ace, Value.Two, Value.Three, Value.Four, Value.Five };
            }

            // Check for regular straight
            for (var i = 0; i < sortedValues.Count - 4; i++)
            {
                var currentVal = sortedValues[i];
                if (sortedValues[i + 1] != currentVal - 1 || sortedValues[i + 2] != currentVal - 2
                                                          || sortedValues[i + 3] != currentVal - 3 ||
                                                          sortedValues[i + 4] != currentVal - 4) continue;
                straightValues = new List<Value>() { sortedValues[i], sortedValues[i + 1], sortedValues[i + 2], sortedValues[i + 3], sortedValues[i + 4] };
                break;
            }

            if(straightValues.Count == 0) return (false, null);
            
            return (true, new List<Card>()
            {
                flushCards.Find(v => v.value == straightValues[0])!,
                flushCards.Find(v => v.value == straightValues[1])!,
                flushCards.Find(v => v.value == straightValues[2])!,
                flushCards.Find(v => v.value == straightValues[3])!,
                flushCards.Find(v => v.value == straightValues[4])!,
            });
        }
        
        public (bool, List<Card>?) HasFourOfAKind()
        {

            foreach (var count in _valueCounts)
            {
                if(count.Value != 4) continue;

                return (true, new List<Card>());
            }

            return (false, null);
            
            var fourOfAKindRank = _valueCounts.FirstOrDefault(x => x.Value == 4).Key;
            if (fourOfAKindRank == null)
            {
                return (false, new List<Card>());
            }
            
            var x = (int)fourOfAKindRank
    
            var fourOfAKindCards = _playersHand.Where(card => card.value == fourOfAKindRank).ToList();
    
            // Get the highest card that is not part of the four of a kind
            var highestCard = _playersHand.Where(card => card.rank != fourOfAKindRank)
                .OrderByDescending(card => card.value)
                .FirstOrDefault();
    
            // Add the highest card to the four of a kind
            fourOfAKindCards.Add(highestCard);
    
            return (true, fourOfAKindCards);
        }

        public (bool, List<byte>?) HasFullHouse()
        {
            var threeOfAKindValues = new List<byte>();
            var pairValues = new List<byte>();
            
            // can ignore case with four of a kind as we will have already returned before calling HasFullHouse()
            foreach (var valueCount in _valueCounts)
            {
                switch (valueCount.Value)
                {
                    case 3:
                        threeOfAKindValues.Add((byte)valueCount.Key);
                        break;
                    case 2:
                        pairValues.Add((byte)valueCount.Key);
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
                    return (true,
                        new List<byte> { threeOfAKindValue, threeOfAKindValue, threeOfAKindValue, pairValue, pairValue });
                }
                case 1 when pairValues.Count == 2:
                    var pairValue1 = pairValues[0];
                    var pairValue2 = pairValues[1];
                    if (pairValue1 > pairValue2)
                    {
                        return (true,
                            new List<byte> { threeOfAKindValues[0], threeOfAKindValues[0], threeOfAKindValues[0], pairValue1, pairValue1 });
                    }
                    return (true,
                        new List<byte> { threeOfAKindValues[0], threeOfAKindValues[0], threeOfAKindValues[0], pairValue2, pairValue2 });
                case 2:
                {
                    var threeOfAKindValue1 = threeOfAKindValues[0];
                    var threeOfAKindValue2 = threeOfAKindValues[1];
                    if (threeOfAKindValue1 > threeOfAKindValue2)
                    {
                        return (true,
                            new List<byte> { threeOfAKindValue1, threeOfAKindValue1, threeOfAKindValue1, threeOfAKindValue2, threeOfAKindValue2 });
                    }
                    return (true,
                        new List<byte> { threeOfAKindValue2, threeOfAKindValue2, threeOfAKindValue2, threeOfAKindValue1, threeOfAKindValue1 });
                }
                default:
                    return (false, null);        
            }
        }

        
        public (bool, List<Card>?, Hand?) HasAnyFlush()
        {
            Suit? flushSuit = null;
            foreach (var count in this._suitCounts)
            {
                if (count.Value < 5) continue;
                flushSuit = count.Key;
                break;
            }
            
            if (flushSuit == null)
            {
                return (false, null, null);
            }
            
            // Get the cards of the flush suit
            var flushCards = _playersHand.Where(card => card.suit == flushSuit).ToList();

            // check for royal flush
            var (hasRoyalFlush, royalFlushCards) = HasRoyalFlush(flushCards);
            if (hasRoyalFlush)
            {
                return (true, royalFlushCards, Hand.RoyalFlush);
            }
            
            // check for straight flush
            var (hasStraightFlush, straightFlushCards) = HasStraightFlush(flushCards);
            if (hasStraightFlush)
            {
                return (true, straightFlushCards, Hand.StraightFlush);
            }
            
            // Return true and the 5 highest cards of the flush suit
            return (true, flushCards.OrderByDescending(card => card.value)
                .Take(5).ToList(), Hand.Flush);
        }


        public (bool, List<Card>) HasStraight()
        {
            var sortedValues = _sortedDescending.Distinct().ToList();
            var straightValues = new List<Value>();
    
            // Check for Ace low straight
            var aceLowStraight = new List<Value>() { Value.Five, Value.Four, Value.Three, Value.Two, Value.Ace };
            if (aceLowStraight.All(sortedValues.Contains))
            {
                straightValues = new List<Value>() { Value.Ace, Value.Two, Value.Three, Value.Four, Value.Five };
            }

            // Check for regular straight
            for (var i = 0; i < sortedValues.Count - 4; i++)
            {
                var currentVal = sortedValues[i];
                if (sortedValues[i + 1] != currentVal - 1 || sortedValues[i + 2] != currentVal - 2
                                                          || sortedValues[i + 3] != currentVal - 3 ||
                                                          sortedValues[i + 4] != currentVal - 4) continue;
                straightValues = new List<Value>() { sortedValues[i], sortedValues[i + 1], sortedValues[i + 2], sortedValues[i + 3], sortedValues[i + 4] };
                break;
            }

            if(straightValues.Count == 0) return (false, new List<Card>());
            
            return (true, new List<Card>()
            {
                _playersHand.Find(v => v.value == straightValues[0])!,
                _playersHand.Find(v => v.value == straightValues[1])!,
                _playersHand.Find(v => v.value == straightValues[2])!,
                _playersHand.Find(v => v.value == straightValues[3])!,
                _playersHand.Find(v => v.value == straightValues[4])!,
            });
        }

    }
}