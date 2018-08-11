using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wordify.Extensions
{
    public class ImageDisplayExtensions
    {
        /// <summary>
        /// This takes in a byte array and converts it into an image.
        /// It stores the image temporarily which is then accessed for display.
        /// </summary>
        /// <param name="byteData">The byte array which is a representation of the image</param>
        public static void DisplayImage(byte[] byteData)
        {
            using (var ms = new MemoryStream(byteData))
            {
                Image image = Image.FromStream(ms);
                image.Save("wwwroot/test.PNG", System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        /// <summary>
        /// This deletes our temporary image.
        /// </summary>
        public static void DeleteImage()
        {
            File.Delete("wwwroot/test.PNG");
        }
    }
}
