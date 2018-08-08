using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        /**
         * Sets Permissions for individual Blob Storage Containers - Required to modify contents.
         */
        public async void SetPermissions()
        {
            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };

            await _Documents.SetPermissionsAsync(permissions);
            await _Images.SetPermissionsAsync(permissions);
        }

        /**
         * Upload Note contents to Blob Storage
         */
        public async Task<Note> Upload(Note newNote, string text, byte[] byteData)
        {
            string blobName = Guid.NewGuid().ToString();

            CloudBlockBlob blobText = _Documents.GetBlockBlobReference(blobName);
            CloudBlockBlob blobImage = _Images.GetBlockBlobReference(blobName);

            await blobText.UploadTextAsync(text);
            await blobImage.UploadFromByteArrayAsync(byteData, 0, byteData.Length);

            newNote.BlobName = blobName;
            return newNote;
        }


        public async Task<string> GetText(Note note)
        {
            try
            {
                return await _Documents.GetBlockBlobReference(note.BlobName).DownloadTextAsync();
            }
            catch (Exception)
            {
                return "Blob does not exist.";
            }
        }


        public async Task<byte[]> GetImage(Note note)
        {
            try
            {
                byte[] byteData = new byte[note.BlobLength];

                int num = await _Images.GetBlockBlobReference(note.BlobName).DownloadToByteArrayAsync(byteData, 0);

                return byteData;
            }
            catch (Exception)
            {
                throw new Exception("Blob does not exist.");
            }
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
