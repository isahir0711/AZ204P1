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
        public IActionResult Get()
        {
            return Ok("Todo bien!");
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            string containerName = "demoimages";
            string localFilePath = "images/tuki.png";
            var blobServiceClient = _fileStorageService.GetBlobServiceClient();
            var containerClient = _fileStorageService.GetContainerClient(blobServiceClient, containerName);

            await _fileStorageService.UploadFileAsync(localFilePath, containerClient);

            return Ok();
        }

        //Azure Functions
        //Deploy a Azure App Service

        //Load iamges into the api
    }
}
