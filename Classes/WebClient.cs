using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace openstig_msg_score.Classes
{
    public static class WebClient 
    {
        public static async Task GetChecklistXML(string id)
        {
            // Create a New HttpClient object and dispose it when done, so the app doesn't leak resources
            using (HttpClient client = new HttpClient())
            {
                // Call asynchronous network methods in a try/catch block to handle exceptions
                try	
                {
                    client.DefaultRequestHeaders.Accept.Clear();

                    client.DefaultRequestHeaders.Add("Accept", "application/xml");
                    string hosturl = Environment.GetEnvironmentVariable("openstig-api-read-server");
                    HttpResponseMessage response = await client.GetAsync(hosturl + "/download/" + id);

                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    ScoringEngine.ScoreChecklist(responseBody);                    
                }  
                catch(HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");	
                    Console.WriteLine("Message :{0} ",e.Message);
                }
            }
        }
    }
}