using Azure.Identity;
using Azure.Storage.Blobs;

namespace Project1.services
{
    public class FileStorage
    {
        public BlobServiceClient GetBlobServiceClient()
        {
            Uri accountUri = new("https://iztestblob.blob.core.windows.net");
            var defAzCreds = new DefaultAzureCredential();
            BlobServiceClient blobServiceClient = new(accountUri, defAzCreds);

            return blobServiceClient;
        }

        public BlobContainerClient GetContainerClient(BlobServiceClient blobServiceClient, string containerName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            return containerClient;
        }

        public async Task<bool> UploadFileAsync(string localFilePath, BlobContainerClient containerClient)
        {
            string fileName = Path.GetFileName(localFilePath);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(localFilePath, true);

            if (!await blobClient.ExistsAsync())
            {
                Console.WriteLine("Error uploading the blob");
            }
            Console.WriteLine("Blob Uploaded");
            return true;

        }
    }
}