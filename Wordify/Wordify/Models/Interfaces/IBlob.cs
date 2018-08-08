using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wordify.Models.Interfaces
{
    public interface IBlob
    {
        void SetPermissions();
        //upload note(Note newNote, image file, text file) => return void
        void Upload(Note newNote, string text, string imagePath);
        //get image by blobName(blobName) => return image file
        Task<ICloudBlob> GetImage(string blobName);
        //get text by blobname(blobName) => return text file
        Task<ICloudBlob> GetText(string blobName);
        //update text(blobName, text file) => retrun void

        //update image(blobName, image file)=> return void

        //delete blob(blobName)=> return void
    }
}
