using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Wordify.Models.Interfaces
{
    public interface IBlob
    {
        // Sets Permissions for individual Blob Storage Containers - Required to modify contents.
        void SetPermissions();

        // Upload Note contents to Blob Storage
        Task Upload(Note newNote, string text, byte[] byteData);

        // Retrieve image from Images Blob
        Task<byte[]> GetImage(Note note);

        // Retrieve text from Documents Blob
        Task<string> GetText(Note note);

        // Update Blob text
        void UpdateText(Note note, string newText);

        // Update Blob image
        void UpdateImage(Note note, byte[] newImage);

        // Remove note image and text from blob storage
        void DeleteBlob(Note note);
    }
}
