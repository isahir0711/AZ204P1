using Azure.Identity;
using Azure.Storage.Blobs;
using Project1.DTOs;

namespace Project1.services
{
    public class FileStorage
    {
        public BlobServiceClient GetBlobServiceClient()
        {
            //     Uri accountUri = new("https://iztestblob.blob.core.windows.net");
            //     var defAzCreds = new DefaultAzureCredential();
            //     BlobServiceClient blobServiceClient = new(accountUri, defAzCreds);

            BlobServiceClient blobServiceClient = new("");

            return blobServiceClient;
        }

        public BlobContainerClient GetContainerClient(BlobServiceClient blobServiceClient, string containerName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            return containerClient;
        }

        public async Task<bool> UploadFileAsync(IFormFile file, BlobContainerClient containerClient)
        {
            string fileName = file.FileName;
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            var stream = file.OpenReadStream();

            await blobClient.UploadAsync(stream, true);

            if (!await blobClient.ExistsAsync())
            {
                Console.WriteLine("Error uploading the blob");
            }
            Console.WriteLine("Blob Uploaded");
            return true;

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