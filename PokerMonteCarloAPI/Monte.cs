using System.Text.Json;

#nullable enable
namespace PokerMonteCarloAPI
{
    public class Monte : IMonte
    {
        public Response Carlo(Request request)
        {
            return new Response
            {
                Id = 1,
                Test = "hello world",
                Test2 = JsonSerializer.Serialize(request)
            };
        }
    }
}