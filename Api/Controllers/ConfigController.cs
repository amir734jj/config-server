using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create()
        {
            var result = await _configLogic.Create();

            return Ok(result);
        }
        
        [HttpGet]
        [Route("{key}")]
        public async Task<IActionResult> Load([FromRoute] string key)
        {
            var result = await _configLogic.Load(key);

            return Ok(result);
        }
        
        [HttpPut]
        [Route("{key}")]
        public async Task<IActionResult> Create([FromRoute]string key, [FromBody] string value)
        {
            await _configLogic.Update(key, value);

            return RedirectToAction("Load", new { key });
        }
    }
}