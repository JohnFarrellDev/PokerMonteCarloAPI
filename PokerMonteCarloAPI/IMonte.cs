#nullable enable
namespace PokerMonteCarloAPI
{
    public interface IMonte
    {
        Response Carlo(Request request, int numberOfSimulations = 10_000);
    }
}