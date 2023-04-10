using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace PokerMonteCarloAPI
{
    public class Player
    {
        public List<Card> PlayersHand { get; }
        private readonly bool _folded;
        private readonly Dictionary<byte, byte> _valueCounts;
        private readonly Dictionary<byte, byte> _suitCounts;
        private readonly List<byte> _sortedDescending;

        public Player(List<Card> playersHand, bool folded)
        {
            PlayersHand = playersHand;
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
            _sortedDescending = playersHand.Select(c => c.Value).OrderByDescending(v => v).ToList();
        }

        // 0 folded
        // 1 highcard
        // 2 pair
        // 3 twopair
        // 4 threeofakind
        // 5 straight
        // 6 flush
        // 7 fullhouse
        // 8 fourofakind
        // 9 straightflush
        // 10 royalflush
        public (byte, List<byte>) CalculateBestHand()
        {
            if(_folded) return (0, new List<byte>());
            
            // calculate the best poker hand possible from the PLayersHand and the table cards
            // return the best hand and the high cards in order

            var (hasFlush, anyFlushHighCards, flushType) = HasAnyFlush();
            
            // calculate if the List<Card> contains a royal flush (DONE)
            if (flushType == 10) return (10, anyFlushHighCards!);
            
            // calculate if the List<Card> contains a straight flush (DONE)
            if (flushType == 9) return (9, anyFlushHighCards!);
            
            // calculate if the List<Card> contains a four of a kind (DONE)
            var (hasFourOfAKind, fourOfAKindHighCards) = HasFourOfAKind();
            if(hasFourOfAKind) return (8, fourOfAKindHighCards!);
            
            // calculate if the List<Card> contains a full house (DONE)
            var (hasFullHouse, fullHouseHighCards) = HasFullHouse();
            if(hasFullHouse) return (7, fullHouseHighCards!);
            
            // calculate if the List<Card> contains a flush (DONE)
            if (hasFlush) return (6, anyFlushHighCards!);
            
            // calculate if the List<Card> contains a straight (DONE)
            var (hasStraight, straightCards) = HasStraight();
            if (hasStraight) return (5, straightCards!);
            
            // calculate if the List<Card> contains a three of a kind
            var (hasThreeOfAKind, threeOfAKindHighCards) = HasThreeOfAKind();
            if(hasThreeOfAKind) return (4, threeOfAKindHighCards!);
            
            // calculate if the List<Card> contains a two pair
            var (hasTwoPair, twoPairHighCards) = HasTwoPair();
            if(hasTwoPair) return (3, twoPairHighCards!);
            
            // calculate if the List<Card> contains a pair
            var (hasPair, pairHighCards) = HasPair();
            if(hasPair) return (2, pairHighCards!);
            
            // calculate if the List<Card> contains a high card
            return (1, _sortedDescending.Take(5).ToList());
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
            var sortedValues = flushCards.Select(c => c.Value).Distinct().OrderByDescending(v => v).ToList();
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
                var kicker = PlayersHand.Select(card => card.Value).Where(value => value != fourOfAKindRank).MaxBy(v => v);

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
        
        public (bool, List<byte>?, byte?) HasAnyFlush()
        {
            // 99 is used to represent no suit being selected (Suit representation of our card, possible values are Clubs (0), Spades (1), Diamonds (2) and Hearts (3))
            byte flushSuit = 99;
            foreach (var count in _suitCounts)
            {
                if (count.Value < 5) continue;
                flushSuit = count.Key;
                break;
            }
            
            if (flushSuit == 99)
            {
                return (false, null, null);
            }
            
            // Get the cards of the flush suit
            var flushCards = PlayersHand.Where(card => card.Suit == flushSuit).ToList();

            // check for royal flush
            var (hasRoyalFlush, royalFlushCards) = HasRoyalFlush(flushCards);
            if (hasRoyalFlush)
            {
                return (true, royalFlushCards, 10);
            }
            
            // check for straight flush
            var (hasStraightFlush, straightFlushCards) = HasStraightFlush(flushCards);
            if (hasStraightFlush)
            {
                return (true, straightFlushCards, 9);
            }
            
            // Return true and the 5 highest cards of the flush suit
            return (true, flushCards.Select(card => card.Value).OrderByDescending(v => v).Take(5).ToList(), 6);
        }


        public (bool, List<byte>?) HasStraight()
        {
            var sortedValues = _sortedDescending.Distinct().ToList();
            
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
                var descendingHighCards = _sortedDescending.Where(value => value != threeOfAKindRank).ToList();
                var kicker1 = descendingHighCards[0];
                var kicker2 = descendingHighCards[1];

                return (true,
                    new List<byte>
                        { threeOfAKindRank, threeOfAKindRank, threeOfAKindRank, kicker1, kicker2 });
            }

            return (false, null);
        }

        // do not bother to check for a value count of 3 or 4 as would have already returned with HasFourOfAKind() or HasThreeOfAKind()
        public (bool, List<byte>?) HasTwoPair()
        {
            var pairValues = new List<byte>();
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
            
            return (true,
                new List<byte>
                    { pairValuesSortedDescending[0], pairValuesSortedDescending[0], pairValuesSortedDescending[1], pairValuesSortedDescending[1], kicker });
        }
        
        // Will have already checked for four of a kind, three of a kind and two pairs
        public (bool, List<byte>?) HasPair()
        {
            foreach (var count in _valueCounts)
            {
                if (count.Value != 2) continue;
                var pairRank = _valueCounts.First(x => x.Value == 2).Key;
                var descendingHighCards = _sortedDescending.Where(value => value != pairRank).ToList();
                var kicker1 = descendingHighCards[0];
                var kicker2 = descendingHighCards[1];
                var kicker3 = descendingHighCards[2];

                return (true,
                    new List<byte>
                        { pairRank, pairRank, kicker1, kicker2, kicker3 });
            }

            return (false, null);
        }
        
    }
}