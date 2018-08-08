using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wordify.Models.Interfaces;

namespace Wordify.Models
{
    public class DevBlob : IBlob
    {
        private CloudStorageAccount _CloudStorageAccount { get; }
        private CloudBlobClient _CloudBlobClient { get; }
        private CloudBlobContainer _Documents { get; }
        private CloudBlobContainer _Images { get; }

        
        public DevBlob(IConfiguration Configuration)
        {
            _CloudStorageAccount = CloudStorageAccount.Parse(Configuration["BlobStorage:ConnectionString"]);
            _CloudBlobClient = _CloudStorageAccount.CreateCloudBlobClient();

            _Documents = _CloudBlobClient.GetContainerReference(Configuration["BlobStorage:DocumentContainer"]);
            _Images = _CloudBlobClient.GetContainerReference(Configuration["BlobStorage:ImageContainer"]);
            SetPermissions();
        }


        public async void SetPermissions()
        {
            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };

            await _Documents.SetPermissionsAsync(permissions);
            await _Images.SetPermissionsAsync(permissions);
        }


        public async void Upload(Note newNote, string text, string imagePath)
        {
            string blobName = Guid.NewGuid().ToString();

            CloudBlockBlob blobText = _Documents.GetBlockBlobReference(blobName);
            CloudBlockBlob blobImage = _Images.GetBlockBlobReference(blobName);

            await blobText.UploadTextAsync(text);
            await blobImage.UploadFromFileAsync(imagePath);

            newNote.BlobName = blobName;
        }


        public async Task<ICloudBlob> GetText(string blobName)
        {
            ICloudBlob textResult = await _Documents.GetBlobReferenceFromServerAsync(blobName);
            return textResult;
        }


        public async Task<ICloudBlob> GetImage(string blobName)
        {
            ICloudBlob imageResult = await _Images.GetBlobReferenceFromServerAsync(blobName);
            return imageResult;
        }




        public async void DeleteBlob(string blobName)
        {
            CloudBlob imageBlob = _Images.GetBlobReference(blobName);
            CloudBlob textBlob = _Documents.GetBlobReference(blobName);

            await imageBlob.DeleteIfExistsAsync();
            await textBlob.DeleteIfExistsAsync();
        }

    }
}
