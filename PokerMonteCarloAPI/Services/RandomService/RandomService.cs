using System;

namespace PokerMonteCarloAPI.Services
{
    public class RandomService : IRandomService
    {
        private Random _random;

        public RandomService()
        {
            _random = new Random();
        }

        public int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        public int Next(int maxValue)
        {
            return _random.Next(maxValue);
        }

        public void SetSeed(int seed)
        {
            _random = new Random(seed);
        }
    }
}