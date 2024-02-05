using ServMidMan.Models;
using System.Drawing;
namespace ServMidMan.Helper
{
    public static class ImageOperator
    {
        public static byte[] ImageToByteArray(IFormFile image)
        {
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    image.Save(ms, image.RawFormat);
            //    return ms.ToArray();
            //}


            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Copy the file content to the memory stream
                image.CopyTo(memoryStream);

                // Convert the memory stream to byte array
                byte[] imageData = memoryStream.ToArray();
                return imageData;

            }

        }
    }
}
