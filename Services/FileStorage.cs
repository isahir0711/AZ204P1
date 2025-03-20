using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Project1.DTOs;
using Project1.Enums;
using Project1.ErrorHandling;

namespace Project1.Services
{
    public class FileStorage
    {

        private const string containerName = "demoimages";
        private const string accountName = "iztestblob";
        private const string accountEnvVarName = "STORAGEKEY";
        public BlobServiceClient GetBlobServiceClient()
        {
            string accountKey = Environment.GetEnvironmentVariable(accountEnvVarName);

            if (string.IsNullOrEmpty(accountKey))
            {
                Console.WriteLine("Woops no account key");
            }
            StorageSharedKeyCredential storageSharedKeyCredential =
                new(accountName, accountKey);
            BlobServiceClient blobServiceClient = new BlobServiceClient(
            new Uri($"https://{accountName}.blob.core.windows.net"),
            storageSharedKeyCredential);

            return blobServiceClient;
        }

        public async Task<Result<Uri>> GetBlobURI(BlobServiceClient blobServiceClient)
        {
            BlobClient blobClient = blobServiceClient
                .GetBlobContainerClient("demoimages")
                .GetBlobClient("obscura_5_desktop.jpg");

            //Todo validate if the blob exists

            var res = await CreateServiceSASBlob(blobClient);

            if (!res.IsSuccess)
            {
                return Result<Uri>.Failure(res.ErrorMessage);
            }

            Uri blobUri = res.Value;

            return Result<Uri>.Success(blobUri);
        }

        public static async Task<Result<Uri>> CreateServiceSASBlob(BlobClient blobClient, string storedPolicyName = null)
        {
            // Check if BlobContainerClient object has been authorized with Shared Key
            if (!blobClient.CanGenerateSasUri)
            {
                return Result<Uri>.Failure("Client object is not authorized via Shared Key");
            }
            // Create a SAS token that's valid for one day
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                BlobName = blobClient.Name,
                Resource = "b"
            };

            if (storedPolicyName == null)
            {
                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(2);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
            }
            else
            {
                sasBuilder.Identifier = storedPolicyName;
            }

            Uri sasURI = blobClient.GenerateSasUri(sasBuilder);

            return Result<Uri>.Success(sasURI);
        }

        public async Task<Result<GetBlobDTO>> UploadFileAsync(IFormFile file, BlobServiceClient blobServiceClient)
        {
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("demoimages");
            long fileSizeBytes = file.Length;
            string fileName = file.FileName;
            string fileExtension = Path.GetExtension(fileName).Substring(1);
            if (!Enum.IsDefined(typeof(SupportedFiles), fileExtension))
            {
                return Result<GetBlobDTO>.Failure("Send an valid image file");
            }
            if (fileSizeBytes > 10_000_000)
            {
                return Result<GetBlobDTO>.Failure("The image size must be lower than 5MB");
            }

            BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

            var stream = file.OpenReadStream();

            var Metadata = new Dictionary<string, string>
                {
                    { "ContentType", file.ContentType },
                };

            BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType };
            await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });

            if (!await blobClient.ExistsAsync())
            {
                return Result<GetBlobDTO>.Failure("The blob didn't upload correctly");
            }

            var getBlobDTO = new GetBlobDTO { BlobName = blobClient.Name };
            return Result<GetBlobDTO>.Success(getBlobDTO);

        }

        public async Task<List<string>> ListBlobs(BlobServiceClient blobServiceClient)
        {
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("demoimages");

            List<string> blobList = [];

            var resSegment = blobContainerClient.GetBlobsAsync();

            await foreach (var blobItem in resSegment)
            {
                blobList.Add(blobItem.Name);
            }

            return blobList;
        }

        public async Task DeleteBlob(string blobName, BlobServiceClient blobServiceClient)
        {
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("demoimages");
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