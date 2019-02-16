using System;
using NATS.Client;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

using openstig_msg_score.Models;
using openstig_msg_score.Classes;

namespace openstig_msg_score
{
    class Program
    {
        static void Main(string[] args)
        {           
            // Create a new connection factory to create
            // a connection.
            ConnectionFactory cf = new ConnectionFactory();

            // Creates a live connection to the default
            // NATS Server running locally
            IConnection c = cf.CreateConnection(Environment.GetEnvironmentVariable("natsserverurl"));

            // Setup an event handler to process incoming messages.
            // An anonymous delegate function is used for brevity.
            EventHandler<MsgHandlerEventArgs> newChecklist = (sender, natsargs) =>
            {
                // print the message
                Console.WriteLine(natsargs.Message.Subject);
                Console.WriteLine(Encoding.UTF8.GetString(natsargs.Message.Data));
                Task<Artifact> checklist = WebClient.GetChecklistXML(Encoding.UTF8.GetString(natsargs.Message.Data));
                if (checklist != null && checklist.Result != null && !string.IsNullOrEmpty(checklist.Result.rawChecklist)){
                    Score score = ScoringEngine.ScoreChecklist(checklist.Result.rawChecklist);          
                    score.title = checklist.Result.title;
                    score.artifactId = checklist.Result.InternalId;
                    score.description = checklist.Result.description;
                    score.created = DateTime.Now;
                    score.SaveScore();
                }
            };
            EventHandler<MsgHandlerEventArgs> update = (sender, natsargs) =>
            {
                // print the message
                Console.WriteLine(natsargs.Message.Subject);
                Console.WriteLine(Encoding.UTF8.GetString(natsargs.Message.Data));
                Task<Artifact> checklist = WebClient.GetChecklistXML(Encoding.UTF8.GetString(natsargs.Message.Data));
                if (checklist != null && checklist.Result != null && !string.IsNullOrEmpty(checklist.Result.rawChecklist)){
                    Score score = ScoringEngine.ScoreChecklist(checklist.Result.rawChecklist);          
                    score.title = checklist.Result.title;
                    score.artifactId = checklist.Result.InternalId;
                    score.description = checklist.Result.description;
                    score.created = DateTime.Now;
                    score.SaveScore();
                }
            };

            // The simple way to create an asynchronous subscriber
            // is to simply pass the event in.  Messages will start
            // arriving immediately.
            IAsyncSubscription asyncNew = c.SubscribeAsync("openstig.save.new", newChecklist);
            IAsyncSubscription asyncUpdate = c.SubscribeAsync("openstig.save.update", update);
        }
    }
}
