
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Project.UI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (HttpClient client = new HttpClient())
            {
                // Change this to your API's actual HTTPS address.
                client.BaseAddress = new Uri("https://localhost:7156/");

                Console.WriteLine("Connecting to Campus IT Asset API...");

                bool connected = false;

                // Wait for the API to become available.
                for (int i = 0; i < 30; i++)
                {
                    try
                    {
                        HttpResponseMessage response =
                            await client.GetAsync("api/asset");

                        if (response.IsSuccessStatusCode)
                        {
                            connected = true;
                            break;
                        }
                    }
                    catch (HttpRequestException)
                    {
                        // API is not ready yet.
                    }

                    await Task.Delay(1000);
                }

                if (connected)
                {
                    Console.Clear();
                    Console.WriteLine("Successfully connected to the API!");

                    MainMenu menu = new MainMenu(client);
                    menu.Display();
                }
                else
                {
                    Console.WriteLine("Unable to connect to the API.");
                    Console.WriteLine("Please check if the backend is running.");
                    Console.ReadKey();
                }
            }
        }
    }
}
