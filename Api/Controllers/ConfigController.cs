using System.IO;
using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers
{
    [Route("api")]
    public class ConfigController : Controller
    {
        private readonly IConfigLogic _configLogic;

        public ConfigController(IConfigLogic configLogic)
        {
            _configLogic = configLogic;
        }

        /// <summary>
        /// Creates a new config entry
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
        /// Loads the config entry
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

            return Ok(result);
        }
        
        /// <summary>
        /// Updated the config entry
        /// </summary>
        /// <param name="key">lookup key</param>
        /// <returns>empty response</returns>
        [HttpPut]
        [Route("{key}")]
        [SwaggerOperation("Update")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Update([FromRoute] string key)
        {
            var value = await new StreamReader(Request.Body).ReadToEndAsync();
            
            await _configLogic.Update(key, value);

            var response = await _configLogic.Load(key);

            return Ok(response);
        }
    }
}