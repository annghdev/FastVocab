using FastVocab.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FastVocab.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileStorageService _service;

        public FilesController(IFileStorageService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var result = await _service.UploadAsync(file, "test", "1");
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Errors);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery]string url)
        {
            var result = await _service.DeleteFileAsync(url);
            if (result.IsSuccess)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }
    }
}
