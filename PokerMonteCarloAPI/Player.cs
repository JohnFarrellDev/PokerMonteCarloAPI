using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace PokerMonteCarloAPI
{
    public class Player
    {
        private readonly List<Card> _playersHand;
        private readonly bool _folded;
        private readonly Dictionary<byte, byte> _valueCounts;
        private readonly Dictionary<byte, byte> _suitCounts;
        private readonly List<byte> _sortedDescending;

        public Player(List<Card> playersHand, bool folded)
        {
            _playersHand = playersHand;
            _folded = folded;
            _valueCounts = new Dictionary<byte, byte>();
            _suitCounts = new Dictionary<byte, byte>();
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
            _sortedDescending = playersHand.Select(c => c.Value).OrderDescending().ToList();
        }

        public (Hand, List<byte>) CalculateBestHand()
        {
            // calculate the best poker hand possible from the PLayersHand and the table cards
            // return the best hand and the high cards in order

            var (hasFlush, anyFlushHighCards, flushType) = HasAnyFlush();
            
            // calculate if the List<Card> contains a royal flush (DONE)
            if (flushType == Hand.RoyalFlush) return (Hand.RoyalFlush, anyFlushHighCards!);
            
            // calculate if the List<Card> contains a straight flush (DONE)
            if (flushType == Hand.StraightFlush) return (Hand.StraightFlush, anyFlushHighCards!);
            
            // calculate if the List<Card> contains a four of a kind (DONE)
            var (hasFourOfAKind, fourOfAKindHighCards) = HasFourOfAKind();
            if(hasFourOfAKind) return (Hand.FourOfAKind, fourOfAKindHighCards!);
            
            // calculate if the List<Card> contains a full house (DONE)
            var (hasFullHouse, fullHouseHighCards) = HasFullHouse();
            if(hasFullHouse) return (Hand.FullHouse, fullHouseHighCards!);
            
            // calculate if the List<Card> contains a flush (DONE)
            if (hasFlush) return (Hand.Flush, anyFlushHighCards!);
            
            // calculate if the List<Card> contains a straight (DONE)
            var (hasStraight, straightCards) = HasStraight();
            if (hasStraight) return (Hand.Straight, straightCards!);
            
            // calculate if the List<Card> contains a three of a kind
            var (hasThreeOfAKind, threeOfAKindHighCards) = HasThreeOfAKind();
            if(hasThreeOfAKind) return (Hand.ThreeOfAKind, threeOfAKindHighCards!);
            
            // calculate if the List<Card> contains a two pair
            
            
            // calculate if the List<Card> contains a pair
            
            
            // calculate if the List<Card> contains a high card
        }

        // flushCards will always be belonging to the same Suit
        private static (bool, List<byte>?) HasRoyalFlush(List<Card> flushCards)
        {
            var royalFlush = new List<byte> { 10,11, 12, 13, 14 };
            var isRoyalFlush = royalFlush.All(v => flushCards.Select(c => c.Value).Contains(v));

            if (isRoyalFlush)
            {
                return (true, new List<byte>
                {
                    14,
                    13,
                    12,
                    11,
                    10,
                });
            }

            return (false, null);
        }

        // flushCards will always be belonging to the same Suit
        public (bool, List<byte>?) HasStraightFlush(List<Card> flushCards)
        {
            var sortedValues = flushCards.Select(c => c.Value).Distinct().OrderByDescending(b => b).ToList();
            // in case of Ace low straight
            if(sortedValues[0] == 14) sortedValues.Add(1);
            
            // Check for regular straight
            for (var i = 0; i < sortedValues.Count - 4; i++)
            {
                var currentVal = sortedValues[i];
                if (sortedValues[i + 1] != currentVal - 1 || sortedValues[i + 2] != currentVal - 2
                                                          || sortedValues[i + 3] != currentVal - 3 ||
                                                          sortedValues[i + 4] != currentVal - 4) continue;
                return (true,
                    new List<byte>
                    {
                        sortedValues[i], sortedValues[i + 1], sortedValues[i + 2], sortedValues[i + 3],
                        sortedValues[i + 4]
                    });
            }
            
            return (false, null);
        }
        
        public (bool, List<byte>?) HasFourOfAKind()
        {
            foreach (var count in _valueCounts)
            {
                if(count.Value != 4) continue;

                var fourOfAKindRank = _valueCounts.FirstOrDefault(x => x.Value == 4).Key;
                var kicker = _playersHand.Where(card => card.Value != fourOfAKindRank).OrderByDescending(b => b)
                    .First().Value;

                return (true,
                    new List<byte>
                        { fourOfAKindRank, fourOfAKindRank, fourOfAKindRank, fourOfAKindRank, kicker });
            }

            return (false, null);
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
        
        public (bool, List<byte>?, Hand?) HasAnyFlush()
        {
            // 5 is used to represent no suit being selected (Suit representation of our card, possible values are Clubs (0), Spades (1), Diamonds (2) and Hearts (3))
            byte flushSuit = 5;
            foreach (var count in _suitCounts)
            {
                if (count.Value < 5) continue;
                flushSuit = count.Key;
                break;
            }
            
            if (flushSuit == 5)
            {
                return (false, null, null);
            }
            
            // Get the cards of the flush suit
            var flushCards = _playersHand.Where(card => card.Suit == flushSuit).ToList();

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
            return (true, straightFlushCards!.OrderByDescending(v => v)
                .Take(5).ToList(), Hand.Flush);
        }


        public (bool, List<byte>?) HasStraight()
        {
            var sortedValues = _playersHand.Select(c => c.Value).Distinct().OrderByDescending(b => b).ToList();
            
            // in case of Ace low straight
            if(sortedValues[0] == 14) sortedValues.Add(1);
            
            // Check for regular straight
            for (var i = 0; i < sortedValues.Count - 4; i++)
            {
                var currentVal = sortedValues[i];
                if (sortedValues[i + 1] != currentVal - 1 || sortedValues[i + 2] != currentVal - 2
                                                          || sortedValues[i + 3] != currentVal - 3 ||
                                                          sortedValues[i + 4] != currentVal - 4) continue;
                return (true,
                    new List<byte>
                    {
                        sortedValues[i], sortedValues[i + 1], sortedValues[i + 2], sortedValues[i + 3],
                        sortedValues[i + 4]
                    });
            }
            
            return (false, null);
        }
        
        // Do not bother to check for pair or additional three of a kind as we would have already checked when calling HasFullHouse()
        public (bool, List<byte>?) HasThreeOfAKind()
        {
            foreach (var count in _valueCounts)
            {
                if(count.Value != 3) continue;

                var threeOfAKindRank = _valueCounts.FirstOrDefault(x => x.Value == 3).Key;
                var descendingHighCards = _playersHand.Where(card => card.Value != threeOfAKindRank).OrderByDescending(b => b).ToList();
                var kicker1 = descendingHighCards[0].Value;
                var kicker2 = descendingHighCards[1].Value;

                return (true,
                    new List<byte>
                        { threeOfAKindRank, threeOfAKindRank, threeOfAKindRank, kicker1, kicker2 });
            }

            return (false, null);
        }

    }
}