using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using FluentValidation;

#nullable enable
namespace PokerMonteCarloAPI
{
    public class Request
    {
        [JsonPropertyName("players")] 
        public List<PlayerRequest> Players { get; init; } = null!;
        
        [JsonPropertyName("tableCards")]
        public List<Card> TableCards { get; set; } = null!;
    }

    public class PlayerRequest
    {
        [JsonPropertyName("cards")]
        public List<Card> Cards { get; set; } = null!;
        
        [JsonPropertyName("folded")]
        public bool Folded { get; set; }
    }
    
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(r => r.Players.Count)
                .GreaterThanOrEqualTo(2)
                .LessThanOrEqualTo(14);

            RuleFor(r => r.TableCards.Count).LessThanOrEqualTo(5);
            
            RuleForEach(r => r.Players)
                .Must(player => player.Cards.Count <= 2)
                .WithMessage("can only provide at most 2 cards for any player");

            RuleFor(r => r.Players.Count(p => !p.Folded))
                .GreaterThanOrEqualTo(1)
                .WithName("Player folded count")
                .WithMessage("Must provide at least one player who has not folded");

            RuleFor(r => r.Players).Must((request, _) =>
                {
                    foreach (var card in request.Players.SelectMany(player => player.Cards))
                    {
                        if ((int)card.Value < 2 || (int)card.Value > 14) return false;
                        if ((int)card.Suit < 0 || (int)card.Suit > 3) return false;
                    }
                    foreach (var card in request.TableCards)
                    {
                        if ((int)card.Value < 2 || (int)card.Value > 14) return false;
                        if ((int)card.Suit < 0 || (int)card.Suit > 3) return false;
                    }
                    return true;
                })
                .WithMessage(
                    "card value must be between 2 and 14 (inclusive), card suit must be between 0 and 3 (inclusive)");
            
            RuleFor(r => r).Must((request, _) =>
                {
                    var providedCardCount = request.TableCards.Count +
                                            request.Players.Sum(requestOpponentsHand =>
                                                requestOpponentsHand.Cards.Count);
            
                    var uniqueProvidedCards = new HashSet<Card>();
                    uniqueProvidedCards.UnionWith(request.TableCards);
                    foreach (var player in request.Players)
                    {
                        uniqueProvidedCards.UnionWith(player.Cards);
                    }
            
                    return uniqueProvidedCards.Count == providedCardCount;
                })
                .WithMessage("provided cards (player and table cards) must all be unique");
        }
    }
}