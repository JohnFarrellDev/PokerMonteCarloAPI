using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

#nullable enable
namespace PokerMonteCarloAPI
{
    [Route("api")]
    [ApiController]
    public class Controller : ControllerBase
    {
        [HttpPost]
        [Produces("application/json")]
        public ActionResult<Response> Test([FromBody] Request request)
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