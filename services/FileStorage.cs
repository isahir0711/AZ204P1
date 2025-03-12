using Azure.Identity;
using Azure.Storage.Blobs;
using Project1.DTOs;
using Project1.Enums;
using Project1.ErrorHandling;

namespace Project1.services
{
    public class FileStorage
    {
        public BlobServiceClient GetBlobServiceClient()
        {
            string blobConnectionString = Environment.GetEnvironmentVariable("BLOBCONN");

            BlobServiceClient blobServiceClient = new(blobConnectionString);

            return blobServiceClient;
        }

        public BlobContainerClient GetContainerClient(BlobServiceClient blobServiceClient, string containerName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            return containerClient;
        }

        public async Task<Result<GetBlobDTO>> UploadFileAsync(IFormFile file, BlobContainerClient blobContainerClient)
        {
            long fileSizeBytes = file.Length;
            string fileName = file.FileName;
            string fileExtension = Path.GetExtension(fileName).Substring(1);
            if (!Enum.IsDefined(typeof(SupportedFiles), fileExtension))
            {
                return Result<GetBlobDTO>.Failure("Send an valid image file");
            }
            if (fileSizeBytes > 5_000_000)
            {
                return Result<GetBlobDTO>.Failure("The image size must be lower than 5MB");
            }

            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

            var stream = file.OpenReadStream();

            await blobClient.UploadAsync(stream, true);

            if (!await blobClient.ExistsAsync())
            {
                return Result<GetBlobDTO>.Failure("The blob didn't upload correctly");
            }

            var getBlobDTO = new GetBlobDTO { BlobName = blobClient.Name };
            return Result<GetBlobDTO>.Success(getBlobDTO);

        }

        public async Task<List<string>> ListBlobs(BlobContainerClient blobContainerClient)
        {
            List<string> blobList = [];

            var resSegment = blobContainerClient.GetBlobsAsync();

            await foreach (var blobItem in resSegment)
            {
                blobList.Add(blobItem.Name);
            }

            return blobList;
        }

        public async Task DownloadBlob(string blobName, BlobContainerClient blobContainerClient)
        {
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
            try
            {
                using var stream = await blobClient.OpenReadAsync();

                FileStream blobStream = File.OpenWrite($"images/{blobClient.Name}");
                await stream.CopyToAsync(blobStream);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task DeleteBlob(string blobName, BlobContainerClient blobContainerClient)
        {
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

            try
            {
                await blobClient.DeleteAsync();
            }
            catch (System.Exception)
            {

                throw;
            }
        }


    }
}