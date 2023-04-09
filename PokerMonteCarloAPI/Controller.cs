using System.Collections.Generic;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PokerMonteCarloAPI
{
    /// <summary>
    /// API controller to call our poker monte carlo functionality
    /// </summary>
    [Route("api")]
    [ApiController]
    public class Controller : ControllerBase
    {
        private readonly IMonte _monte;
        
        public Controller(IMonte monte)
        {
            _monte = monte;
        }

        [Route("monte")]
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<PlayerResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status400BadRequest)]
        public IActionResult MonteCarlo([FromBody] Request request)
        {
            var result = _monte.Carlo(request);
            return Ok(result);
        }
    }
}