using System;
using System.Collections.Generic;
using FluentValidation;
using System.Linq;

#nullable enable
namespace PokerMonteCarloAPI
{
    public class Request
    {
        public required List<PlayerRequest> Players { get; set; }
        public required List<Card> TableCards { get; set; }
    }

    public class PlayerRequest
    {
        public required List<Card> Cards { get; set; }
        public required bool Folded { get; set; }
    }
    
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {

            RuleFor(r => r.Players.Count(p => !p.Folded))
                .GreaterThanOrEqualTo(1)
                .WithName("Active player count")
                .WithMessage("Must provide at least one player who has not folded");

            RuleFor(r => r.Players.Count)
                .GreaterThanOrEqualTo(2)
                .LessThanOrEqualTo(14);

            RuleFor(r => r.Players)
                .NotNull().WithMessage("Players list cannot be null")
                .NotEmpty().WithMessage("Players list cannot be empty");

            RuleFor(r => r.TableCards)
                .NotNull().WithMessage("Table cards list cannot be null")
                .Must(cards => cards.Count <= 5)
                .WithMessage("'Table Cards Count' must be less than or equal to '5'.");

            When(r => r.Players != null, () =>
            {
                RuleForEach(r => r.Players).SetValidator(new PlayerRequestValidator());
            });

            When(r => r.TableCards != null, () =>
            {
                RuleForEach(r => r.TableCards).SetValidator(new CardValidator());
            });

            RuleFor(r => r)
                .Must(r => !r.Players.Any(p => p.Cards.Any(c => r.TableCards.Contains(c))))
                .WithMessage("Player cards must not be present in the table cards");

            RuleFor(r => r)
                .Must(r => {
                    var allPlayerCards = r.Players.SelectMany(p => p.Cards).ToList();
                    return allPlayerCards.Count == allPlayerCards.Distinct().Count();
                })
                .WithMessage("Players cannot share cards");
        }
    }

    public class PlayerRequestValidator : AbstractValidator<PlayerRequest>
    {
        public PlayerRequestValidator()
        {
            RuleFor(p => p.Cards)
                .NotNull().WithMessage("Player cards cannot be null")
                .Must(cards => cards != null && cards.Count == 2)
                .WithMessage("Each player must have exactly 2 cards");

            RuleFor(p => p.Cards)
                .ForEach(card => card.SetValidator(new CardValidator()));
        }
    }

    public class CardValidator : AbstractValidator<Card>
    {
        public CardValidator()
        {
            RuleFor(c => c.Suit)
                .IsInEnum()
                .WithMessage(c => 
                {
                    var allowedSuits = Enum.GetValues(typeof(Suit))
                        .Cast<Suit>()
                        .Select(s => $"{(int)s} ({s})")
                        .ToList();

                    return $"Invalid card suit: {c.Suit}. Allowed suits are: {string.Join(", ", allowedSuits)}";
                });

            RuleFor(c => c.Value)
                .IsInEnum()
                .WithMessage(c => 
                {
                    var allowedValues = Enum.GetValues(typeof(Value))
                        .Cast<Value>()
                        .Select(v => $"{(int)v} ({v})")
                        .ToList();

                    return $"Invalid card value: {c.Value}. Allowed values are: {string.Join(", ", allowedValues)}";
                });
        }
    }
}