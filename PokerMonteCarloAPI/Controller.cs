using Microsoft.AspNetCore.Mvc;

#nullable enable
namespace PokerMonteCarloAPI
{
    [Route("api")]
    [ApiController]
    public class Controller : ControllerBase
    {
        private readonly IMonte _monte;

        public Controller(IMonte monte)
        {
            _monte = monte;
        }

        [HttpPost]
        [Produces("application/json")]
        public ActionResult<Response> Test([FromBody] Request request)
        {
            var result = _monte.Carlo(request);
            return Ok(result);
        }
    }
}