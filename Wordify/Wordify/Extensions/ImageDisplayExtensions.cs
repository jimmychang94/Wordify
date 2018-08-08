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
        public static void DisplayImage(byte[] byteData)
        {
            using (var ms = new MemoryStream(byteData))
            {
                Image image = Image.FromStream(ms);
                image.Save("wwwroot/test.PNG", System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        public static void DeleteImage()
        {
            File.Delete("wwwroot/test.PNG");
        }
    }
}
