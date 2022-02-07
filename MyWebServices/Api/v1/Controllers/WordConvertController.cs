using Microsoft.AspNetCore.Mvc;
using MyWebServices.Core.DataAccess.Repositories;
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

        [HttpPost("process-file")]
        public IActionResult Post(IFormFile File)
        {
            if (File.Length == 0) return BadRequest("Пустой файл.");

            var stream = File.OpenReadStream();
            var memoryStream = new MemoryStream();
            while (true)
            {
                var b = stream.ReadByte();
                if (b == -1) break;
                memoryStream.WriteByte((byte)b);
            }
           
            var wordManager = new WordManager(memoryStream, _userRepository.GetUserSettings(1, 2));
            var convertedText = wordManager.GetCovertedText();

            _logger.LogInformation($"File uploaded and converted. Converted text: {convertedText}");
            return Ok(convertedText);
        }
    }
}