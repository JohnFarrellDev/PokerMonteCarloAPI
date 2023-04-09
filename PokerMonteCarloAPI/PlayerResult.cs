using Newtonsoft.Json;

namespace PokerMonteCarloAPI
{
    public class PlayerResult
    {
        
        [JsonProperty("winCount")]
        public int winCount { get; set; }
        
        [JsonProperty("loseCount")]
        public int loseCount { get; set; }
        
        [JsonProperty("twoWayTieCount")]
        public int twoWayTieCount { get; set; }
        
        [JsonProperty("threeWayTieCount")]
        public int threeWayTieCount { get; set; }
        
        [JsonProperty("fourWayTieCount")]
        public int fourWayTieCount { get; set; }
        
        [JsonProperty("fiveWayTieCount")]
        public int fiveWayTieCount { get; set; }
        
        [JsonProperty("sixWayTieCount")]
        public int sixWayTieCount { get; set; }
        
        [JsonProperty("sevenWayTieCount")]
        public int sevenWayTieCount { get; set; }
        
        [JsonProperty("eightWayTieCount")]
        public int eightWayTieCount { get; set; }
        
        [JsonProperty("nineWayTieCount")]
        public int nineWayTieCount { get; set; }
    }
}