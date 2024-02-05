using Microsoft.AspNetCore.Http;
using ServMidMan.Models;
using System.Drawing;
using System.Net;
namespace ServMidMan.Helper
{
    public static class ImageOperator
    {
        private static string ftpServerUrl = "ftp://salmadevelop.eu/ServMidMan/";
        private static string userName = "hojszi.salmadevelop.eu";
        private static string password = "myNewDatabasePassword1";

        public static bool imageUploaderToServer(List<IFormFile> fromFile, List<Models.Image> images)
        {

            if (fromFile != null && fromFile.Count > 0)
            {
                for (int i = 0; i < fromFile.Count; i++) 
                {
                    // Create FTP request
                    FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ftpServerUrl + images[i].FileName);
                    ftpRequest.Credentials = new NetworkCredential(userName, password);
                    ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                    // Upload the file to FTP server
                    using (Stream requestStream = ftpRequest.GetRequestStream())
                    {
                        fromFile[i].CopyTo(requestStream);
                    }
                }

            }
            return true;
        }
        public static List<byte[]> DownlaodImages(List<string> imageNames)
        {
            List<byte[]> bytesofImages = new List<byte[]>();
            foreach (var picturename in imageNames)
            {
                // Download the image from FTP server to cache
                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(userName, password);
                    bytesofImages.Add(client.DownloadData(ftpServerUrl + picturename));
                }
                
            }

            return bytesofImages;
        }
    }
}
