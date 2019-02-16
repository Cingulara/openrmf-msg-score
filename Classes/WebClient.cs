using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using openstig_msg_score.Models;
using System.Xml.Serialization;
using System.IO;

namespace openstig_msg_score.Classes
{
    public static class WebClient 
    {
        public static async Task<Artifact> GetChecklistXML(string id)
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
                    Console.WriteLine("URL: {0}", hosturl + "/" + id);
                    HttpResponseMessage response = await client.GetAsync(hosturl + "/" + id);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    XmlSerializer serializer = new XmlSerializer(typeof(Artifact));
                    Artifact art;
                    using (TextReader reader = new StringReader(responseBody))
                    {
                        art = (Artifact)serializer.Deserialize(reader);
                    }
                    // Score score = ScoringEngine.ScoreChecklist(responseBody);                    
                    // score.title = art.title;
                    // score.artifactId = art.InternalId;
                    // score.description = art.description;
                    // score.created = DateTime.Now;
                    // score.SaveScore();
                    return art;
                }
                catch(HttpRequestException e)
                {
                    Console.WriteLine("\nHTTP Exception Caught!");
                    Console.WriteLine("Message :{0}", e.Message );
                    Console.WriteLine("Stack :{0}", e.StackTrace);
                    //throw e;
                    return null;
                }
                catch (Exception ex) {
                    Console.WriteLine("\nGeneral  exception Caught!");	
                    Console.WriteLine("Message :{0}", ex.Message);
                    //throw ex;
                    return null;
                }
            }
        }
    }
}