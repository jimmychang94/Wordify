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
        public async Task Upload(Note newNote, string text, byte[] byteData)
        {
            string blobName = Guid.NewGuid().ToString();

            var blobText = _Documents.GetBlockBlobReference(blobName);
            var blobImage = _Images.GetBlockBlobReference(blobName);

            await blobText.UploadTextAsync(text);
            await blobImage.UploadFromByteArrayAsync(byteData, 0, byteData.Length);

            newNote.BlobName = blobName;
        }


        /**
         * Retrieve text from Documents Blob
         */
        public async Task<string> GetText(Note note)
        {
            try
            {
                return await _Documents.GetBlockBlobReference(note.BlobName).DownloadTextAsync();
            }
            catch (Exception)
            {
                throw new Exception("Blob does not exist");
            }
        }


        /**
         * Retrieve image from Images Blob
         */
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


        /**
         * Update Blob text
         */
        public async void UpdateText(Note note, string newText)
        {
            try
            {
                var textBlob = _Documents.GetBlockBlobReference(note.BlobName);

                await textBlob.UploadTextAsync(newText);
            }
            catch (Exception)
            {
                throw new Exception("Blob does not exist.");
            }

        }


        /**
         * Update Blob image
         */
        public async void UpdateImage(Note note, byte[] newImage)
        {
            try
            {
                var imageBlob = _Documents.GetBlockBlobReference(note.BlobName);

                await imageBlob.UploadFromByteArrayAsync(newImage, 0, newImage.Length);
            }
            catch (Exception)
            {
                throw new Exception("Blob does not exist.");
            }
        }


        /**
         * Remove note image and text from blob
         */
        public async void DeleteBlob(Note note)
        {
            try
            {
                var imageBlob = _Images.GetBlockBlobReference(note.BlobName);
                var textBlob = _Documents.GetBlockBlobReference(note.BlobName);

                await imageBlob.DeleteIfExistsAsync();
                await textBlob.DeleteIfExistsAsync();
            }
            catch (Exception)
            {
                throw new Exception("Blob does not exist.");
            }
        }
    }
}
