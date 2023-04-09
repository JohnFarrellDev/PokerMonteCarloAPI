using Newtonsoft.Json;

namespace PokerMonteCarloAPI
{
    public class PlayerResult
    {
        
        [JsonProperty("winCount")]
        public int WinCount { get; set; }
        
        [JsonProperty("loseCount")]
        public int LoseCount { get; set; }
        
        [JsonProperty("twoWayTieCount")]
        public int TwoWayTieCount { get; set; }
        
        [JsonProperty("threeWayTieCount")]
        public int ThreeWayTieCount { get; set; }
        
        [JsonProperty("fourWayTieCount")]
        public int FourWayTieCount { get; set; }
        
        [JsonProperty("fiveWayTieCount")]
        public int FiveWayTieCount { get; set; }
        
        [JsonProperty("sixWayTieCount")]
        public int SixWayTieCount { get; set; }
        
        [JsonProperty("sevenWayTieCount")]
        public int SevenWayTieCount { get; set; }
        
        [JsonProperty("eightWayTieCount")]
        public int EightWayTieCount { get; set; }
        
        [JsonProperty("nineWayTieCount")]
        public int NineWayTieCount { get; set; }
    }
}