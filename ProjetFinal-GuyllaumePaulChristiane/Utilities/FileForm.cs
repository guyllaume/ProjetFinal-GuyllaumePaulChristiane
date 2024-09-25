using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Drawing.Imaging;

namespace ProjetFinal_GuyllaumePaulChristiane.Utilities
{
    public static class FileForm
    {
        public static IFormFile CreerIFormFile(string content, string filename)
        {
            //Convertir le string en un tableau de bytes
            byte[] bytes = Encoding.UTF8.GetBytes(content);

            //Créer un MemoryStream à partir du tableau de bytes
            MemoryStream stream = new MemoryStream(bytes);

            //Créer un IFormFile à partir du MemoryStream
            IFormFile file = new FormFile(stream, 0, bytes.Length, "name", filename)
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            return file;
        }

        // Load image from a file path
        public static byte[] LoadImageFromPath(string imagePath)
        {
            using (Image image = Image.FromFile(imagePath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Png); // Save as PNG or other format
                    return ms.ToArray();
                }
            }
        }

        // Convert uploaded IFormFile to byte array
        public static byte[] ConvertIFormFileToByteArray(IFormFile formFile)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                formFile.CopyTo(ms);
                return ms.ToArray();
            }
        }

        // Compare two byte arrays
        public static bool CompareImages(byte[] image1, byte[] image2)
        {
            if (image1.Length != image2.Length)
            {
                return false;
            }
            for (int i = 0; i < image1.Length; i++)
            {
                if (image1[i] != image2[i])
                {
                    return false;
                }
            }
            return true;
        }

    }
}
