using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Project1.DTOs;
using Project1.Enums;
using Project1.services;

namespace Project1.Controllers
{
    //Validations, upload only images/types (png,jpg,webp,etc), Size Validation
    //Result Pattern
    //Connection String Env variable (App service)
    //Azure Functions, create thumbnails(Blob trigger), delete blobs after 1 days of creation

    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly FileStorage _fileStorageService;
        private const string containerName = "demoimages";

        public FilesController(FileStorage fileStorage)
        {
            _fileStorageService = fileStorage;
        }

        [HttpGet]
        public async Task<ActionResult<List<string>>> Get()
        {
            var blobServiceClient = _fileStorageService.GetBlobServiceClient();
            var containerClient = _fileStorageService.GetContainerClient(blobServiceClient, containerName);

            var listres = await _fileStorageService.ListBlobs(containerClient);

            return Ok(listres);

        }

        [HttpGet]
        [Route("{blobName}")]
        public async Task<ActionResult> Get(string blobName)
        {
            var blobServiceClient = _fileStorageService.GetBlobServiceClient();
            var containerClient = _fileStorageService.GetContainerClient(blobServiceClient, containerName);

            await _fileStorageService.DownloadBlob(blobName, containerClient);
            Console.WriteLine("downloaded");
            return Ok();
        }

        [HttpDelete]
        [Route("{blobName}")]
        public async Task<ActionResult> Delete(string blobName)
        {

            var blobServiceClient = _fileStorageService.GetBlobServiceClient();
            var containerClient = _fileStorageService.GetContainerClient(blobServiceClient, containerName);

            await _fileStorageService.DeleteBlob(blobName, containerClient);
            Console.WriteLine("Deleted");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post(UploadFileDTO uploadFileDTO)
        {

            var blobServiceClient = _fileStorageService.GetBlobServiceClient();
            var containerClient = _fileStorageService.GetContainerClient(blobServiceClient, containerName);

            var res = await _fileStorageService.UploadFileAsync(uploadFileDTO.File, containerClient);

            if (!res.IsSuccess)
            {
                return BadRequest(res.ErrorMessage);
            }

            return Ok(res.Value);
        }
    }
}
