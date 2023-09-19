using System;
using System.IO;
using System.Net;

namespace AuthenticationRequest
{
    class Program
    {
        static void Main(string[] args)
        {
            CreatioLogin login = new CreatioLogin("https://01195748-5-demo.creatio.com/", "Supervisor", "Supervisor");
            
            try
            {
                var cookie = login.TryLogin();
                var request = login.CreateRequest("https://01195748-5-demo.creatio.com/0/odata/Contact?$top=1", string.Empty, "GET");

                request.CookieContainer = cookie;

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            var responseMessage = reader.ReadToEnd();
                            string filePath = Path.Combine("E:\\FE\\projects\\c#test\\", "response.txt");

                            File.WriteAllText(filePath, responseMessage);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"HTTP Status Code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
