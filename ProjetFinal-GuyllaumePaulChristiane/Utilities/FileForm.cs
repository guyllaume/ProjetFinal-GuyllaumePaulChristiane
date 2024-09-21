using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;

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

    }
}
