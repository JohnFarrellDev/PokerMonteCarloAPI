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
        public byte Suit { get; }
        
        /// <summary>
        /// Represents our card value, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10, Jack = 11, Queen = 12, King = 13, Ace = 14 
        /// </summary>
        [JsonPropertyName("value")]
        public byte Value { get; }
        
        [JsonConstructor]
        public Card(byte value, byte suit)
        {
            Value = value;
            Suit = suit;
        }
        
        protected bool Equals(Card other)
        {
            return Suit == other.Suit && Value == other.Value;
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
            return HashCode.Combine(Suit, Value);
        }
    }
    
    public enum Suit
    {
        Clubs,
        Spades,
        Diamonds,
        Hearts
    }
}