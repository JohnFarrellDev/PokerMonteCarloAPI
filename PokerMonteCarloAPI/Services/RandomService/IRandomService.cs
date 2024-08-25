namespace PokerMonteCarloAPI.Services
{
    public interface IRandomService
    {
        int Next(int maxValue);
        int Next(int minValue, int maxValue);
        void SetSeed(int seed);
    }
}