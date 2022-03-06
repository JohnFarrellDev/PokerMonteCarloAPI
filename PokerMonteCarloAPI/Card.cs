using System;
using System.Text.Json.Serialization;

#nullable enable
namespace PokerMonteCarloAPI
{
    public class Card
    {
        /// <summary>
        /// Suit representation of our card, possible values are Clubs (0), Spades (1), Diamonds (2) and Hearts (3)
        /// </summary>
        [JsonPropertyName("suit")]
        public Suit suit { get; set; }
        
        /// <summary>
        /// Represents our card value, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10, Jack = 11, Queen = 12, King = 13, Ace = 14 
        /// </summary>
        [JsonPropertyName("value")]
        public Value value { get; set; }
        
        [JsonConstructor]
        public Card(Value value, Suit suit)
        {
            this.value = value;
            this.suit = suit;
        }
        
        protected bool Equals(Card other)
        {
            return suit == other.suit && value == other.value;
        }
        
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Card)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)suit, (int)value);
        }
    }
    
    public enum Suit
    {
        Clubs,
        Spades,
        Diamonds,
        Hearts
    }

    public enum Value
    {
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        Ace = 14
    }
}