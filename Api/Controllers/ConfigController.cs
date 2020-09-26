using System.IO;
using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers
{
    [Route("api/v1")]
    public class ConfigController : Controller
    {
        private readonly IConfigLogic _configLogic;

        public ConfigController(IConfigLogic configLogic)
        {
            _configLogic = configLogic;
        }

        /// <summary>
        /// Creates a new config entry and returns the apiKey
        /// </summary>
        /// <returns>Key to lookup the entry</returns>
        [HttpPost]
        [Route("")]
        [SwaggerOperation("Create")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> Create()
        {
            var result = await _configLogic.Create();

            return Ok(result);
        }
        
        /// <summary>
        /// Loads the config entry given an apiKey
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Value of config entry</returns>
        [HttpGet]
        [Route("{key}")]
        [SwaggerOperation("Load")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> Load([FromRoute] string key)
        {
            var result = await _configLogic.Load(key);

            return result == null ? (IActionResult) NotFound() : Ok(result);
        }
        
        /// <summary>
        /// Updates the config entry given an apiKey
        /// </summary>
        /// <param name="key">lookup key</param>
        /// <returns>Updated config entry</returns>
        [HttpPut]
        [Route("{key}")]
        [SwaggerOperation("Update")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Update([FromRoute] string key)
        {
            var value = await new StreamReader(Request.Body).ReadToEndAsync();

            if (value.Length * sizeof(char) > 32000)
            {
                return BadRequest();
            }
            
            var result = await _configLogic.Update(key, value);

            return result == null ? (IActionResult) NotFound() : Ok(result);
        }
        
        /// <summary>
        /// Deletes the config entry
        /// </summary>
        /// <param name="key">lookup key</param>
        /// <returns>Empty response</returns>
        [HttpDelete]
        [Route("{key}")]
        [SwaggerOperation("Delete")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Delete([FromRoute] string key)
        {
            await _configLogic.Delete(key);

            return NoContent();
        }
        
        /// <summary>
        /// Status of server
        /// </summary>
        /// <returns>Empty response</returns>
        [HttpGet]
        [Route("status")]
        [SwaggerOperation("Status")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Status()
        {
            var status = await _configLogic.Status();

            return Ok(status);
        }
    }
}
