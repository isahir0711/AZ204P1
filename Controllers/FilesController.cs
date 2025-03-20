using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project1.DTOs;
using Project1.Services;

namespace Project1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly FileStorage _fileStorageService;
        public FilesController(FileStorage fileStorage)
        {
            _fileStorageService = fileStorage;
        }

        [HttpGet]
        public async Task<ActionResult<List<string>>> Get()
        {
            var blobServiceClient = _fileStorageService.GetBlobServiceClient();
            var listres = await _fileStorageService.ListBlobs(blobServiceClient);

            return Ok(listres);

        }

        [HttpGet]
        [Route("{blobName}")]
        public async Task<ActionResult> Get(string blobName)
        {
            BlobServiceClient blobServiceClient = _fileStorageService.GetBlobServiceClient();
            var res = await _fileStorageService.GetBlobURI(blobServiceClient, blobName);
            if (!res.IsSuccess)
            {
                return BadRequest(res.ErrorMessage);
            }
            Uri blobUri = res.Value;
            return Ok(blobUri);
        }

        [HttpDelete]
        [Route("{blobName}")]
        public async Task<ActionResult> Delete(string blobName)
        {
            var blobServiceClient = _fileStorageService.GetBlobServiceClient();
            await _fileStorageService.DeleteBlob(blobName, blobServiceClient);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post(UploadFileDTO uploadFileDTO)
        {
            var blobServiceClient = _fileStorageService.GetBlobServiceClient();
            var res = await _fileStorageService.UploadFileAsync(uploadFileDTO.File, blobServiceClient);

            if (!res.IsSuccess)
            {
                return BadRequest(res.ErrorMessage);
            }

            return Ok(res.Value);
        }
    }
}
