using Microsoft.AspNetCore.Http;
using ServMidMan.Models;
using System.Drawing;
using System.Net;
namespace ServMidMan.Helper
{
    public static class ImageOperator
    {
        private static string ImageBasePath = "https://servmidman.gugar.sk/";
		private static string ftpServerUrl = "ftp://gugar.sk/gugar.sk/sub/servmidman";
		private static string userName = "hojszi.gugar.sk";
		private static string password = "myNewDatabasePassword1";

		public static bool ImageUploaderToServer(List<IFormFile> fromFiles, List<string> images)
		{
			if (fromFiles != null && fromFiles.Count > 0)
			{
				for (int i = 0; i < fromFiles.Count; i++)
				{
					// Construct full FTP path
					string ftpFilePath = $"{ftpServerUrl}/{images[i]}";

					// Create FTP request
					FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ftpFilePath);
                    ftpRequest.Credentials = new NetworkCredential(userName, password);
					ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

					// Upload the file to FTP server
					using (Stream requestStream = ftpRequest.GetRequestStream())
					{
						fromFiles[i].CopyTo(requestStream);
					}
				}
				return true;
			}
			else
			{
				return false;
			}
		}
        public static void FTPImgaeRemover(List<string> imageNames)
        {
            foreach (var imageName in imageNames)
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpServerUrl + imageName);
                request.Method = WebRequestMethods.Ftp.DeleteFile;

                request.Credentials = new NetworkCredential(userName, password);

                try
                {
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    Console.WriteLine($"FTP Server Response: {response.StatusDescription}");

                    response.Close();
                }
                catch (WebException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
           
        }

        public static List<string> getImageFullPath(List<string> myImages)
        {
            List<string> imgaePaths = new List<string>();
            foreach(var imageName in myImages)
            {
                imgaePaths.Add(ImageBasePath + imageName);
            }
            return imgaePaths;
        }
    }
}
