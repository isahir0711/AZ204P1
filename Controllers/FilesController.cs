using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project1.services;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly FileStorage _fileStorageService;

        public FilesController(FileStorage fileStorage)
        {
            _fileStorageService = fileStorage;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string containerName = "demoimages";
            string localFilePath = "images/tuki.png";
            var blobServiceClient = _fileStorageService.GetBlobServiceClient();
            var containerClient = _fileStorageService.GetContainerClient(blobServiceClient, containerName);

            await _fileStorageService.UploadFileAsync(localFilePath, containerClient);

            return Ok();
        }
    }
}
