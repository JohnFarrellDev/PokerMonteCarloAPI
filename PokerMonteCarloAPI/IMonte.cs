#nullable enable
using System.Collections.Generic;

namespace PokerMonteCarloAPI
{
    public interface IMonte
    {
        List<PlayerResult> Carlo(Request request);
    }
}