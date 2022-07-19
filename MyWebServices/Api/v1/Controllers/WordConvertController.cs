using Microsoft.AspNetCore.Mvc;
using MyWebServices.Core.DataAccess.Entities;
using MyWebServices.Core.DataAccess.Repositories;
using MyWebServices.Core.Models;
using MyWebServices.Core.Services;

namespace MyWebServices.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WordConvertController : ControllerBase
    {
        private readonly ILogger<WordConvertController> _logger;
        private readonly UserRepository _userRepository;

        public WordConvertController(ILogger<WordConvertController> logger, UserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        [HttpPost("process-file/{patternId}")]
        public IActionResult Post(IFormFile file, int patternId)
        {
            if (file.Length == 0) return BadRequest("Пустой файл.");

            var msWordItems = new MsWordReader().GetContent(file.OpenReadStream());
            var wordTextConverter = new WordTextConverter(_userRepository.GetUserSettings(1, patternId));

            return Ok(wordTextConverter.Convert(msWordItems));
        }

        [HttpGet("settings")]
        public IActionResult Get()
        {
            return Ok(_userRepository.GetUserSettingsForView(1));
        }

        [HttpPatch("settings/save")]
        public IActionResult Patch(UserSettingsEntity userSettingsEntity)
        {

            return Ok(_userRepository.UpdateUserSettings(userSettingsEntity, 1));
        }
    }
}