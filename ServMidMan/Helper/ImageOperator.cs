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
				// Handle case where no files are provided
				return false;
			}
		}

		//public static List<byte[]> DownloadImages(List<string> imageNames)
  //      {
  //          List<byte[]> bytesofImages = new List<byte[]>();

  //          // Set TLS version explicitly
  //          ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

  //          foreach (var picturename in imageNames)
  //          {
  //              try
  //              {
  //                  // Download the image from FTP server to cache
  //                  using (WebClient client = new WebClient())
  //                  {
  //                      client.Credentials = new NetworkCredential(userName, password);
  //                      bytesofImages.Add(client.DownloadData(ftpServerUrl + picturename));
  //                  }
  //              }
  //              catch (WebException ex)
  //              {
  //                  // Handle specific FTP errors
  //                  var response = ex.Response as FtpWebResponse;
  //                  if (response != null && response.StatusCode == FtpStatusCode.NotLoggedIn)
  //                  {
  //                      // Handle not logged in error
  //                      Console.WriteLine($"Error downloading image {picturename}: Not logged in.");
  //                  }
  //                  else
  //                  {
  //                      // Handle other FTP errors
  //                      Console.WriteLine($"Error downloading image {picturename}: {ex.Message}");
  //                  }
  //              }
  //              catch (Exception ex)
  //              {
  //                  // Handle general errors
  //                  Console.WriteLine($"Error downloading image {picturename}: {ex.Message}");
  //              }
  //          }

  //          return bytesofImages;
  //      }
        public static void FTPImgaeRemover(List<string> imageNames)
        {
            foreach (var imageName in imageNames)
            {
                // Create FTP request
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpServerUrl + imageName);
                request.Method = WebRequestMethods.Ftp.DeleteFile;

                // Set credentials
                request.Credentials = new NetworkCredential(userName, password);

                try
                {
                    // Send the request to the FTP server
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                    // Display server response
                    Console.WriteLine($"FTP Server Response: {response.StatusDescription}");

                    // Clean up resources
                    response.Close();
                }
                catch (WebException ex)
                {
                    // Handle any errors
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
