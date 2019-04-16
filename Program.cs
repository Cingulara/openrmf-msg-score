using System;
using NATS.Client;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using NLog;
using NLog.Config;
using openstig_msg_score.Models;
using openstig_msg_score.Classes;

using MongoDB.Bson;

namespace openstig_msg_score
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.Configuration = new XmlLoggingConfiguration($"{AppContext.BaseDirectory}nlog.config");

            var logger = LogManager.GetLogger("openrmf-msg-score");
            //logger.Info("log info");
            //logger.Debug("log debug");

            // Create a new connection factory to create a connection.
            ConnectionFactory cf = new ConnectionFactory();

            // Creates a live connection to the default NATS Server running locally
            IConnection c = cf.CreateConnection(Environment.GetEnvironmentVariable("natsserverurl"));

            // Setup an event handler to process incoming messages.
            // An anonymous delegate function is used for brevity.
            EventHandler<MsgHandlerEventArgs> newChecklistScore = (sender, natsargs) =>
            {
                try {
                    // print the message
                    logger.Info("New NATS subject: {0}", natsargs.Message.Subject);
                    logger.Info("New NATS data: {0}",Encoding.UTF8.GetString(natsargs.Message.Data));
                    Artifact checklist = WebClient.GetChecklistAsync(Encoding.UTF8.GetString(natsargs.Message.Data)).GetAwaiter().GetResult();
                    if (checklist != null && checklist.CHECKLIST != null) {
                        Score score = ScoringEngine.ScoreChecklist(checklist.CHECKLIST);
                        score.system = checklist.system;
                        score.stigType = checklist.stigType;
                        score.stigRelease = checklist.stigRelease;
                        score.artifactId = GetInternalId(Encoding.UTF8.GetString(natsargs.Message.Data));
                        score.hostName = checklist.hostName;
                        score.created = DateTime.Now;
                        logger.Info("Saving new score for artifactId {0}", score.artifactId.ToString());
                        score.SaveScore();
                    }
                }
                catch (Exception ex) {
                    // log it here
                    logger.Error(ex, "Error saving new scoring information for artifactId {0}", Encoding.UTF8.GetString(natsargs.Message.Data));
                }
            };

            EventHandler<MsgHandlerEventArgs> updateChecklistScore = (sender, natsargs) =>
            {
                try {
                    // print the message
                    Console.WriteLine(natsargs.Message.Subject);
                    Console.WriteLine(Encoding.UTF8.GetString(natsargs.Message.Data));
                    Artifact checklist = WebClient.GetChecklistAsync(Encoding.UTF8.GetString(natsargs.Message.Data)).GetAwaiter().GetResult();
                    if (checklist != null && checklist.CHECKLIST != null) {
                        Score score = ScoringEngine.ScoreChecklist(checklist.CHECKLIST);   
                        score.system = checklist.system;
                        score.stigType = checklist.stigType;
                        score.stigRelease = checklist.stigRelease;
                        score.created = checklist.created;
                        score.artifactId = GetInternalId(Encoding.UTF8.GetString(natsargs.Message.Data));
                        score.hostName = checklist.hostName;
                        score.updatedOn = DateTime.Now;
                        logger.Info("Saving updated score for artifactId {0}", score.artifactId.ToString());
                        score.UpdateScore();
                    }
                }
                catch (Exception ex) {
                    // log it here
                    logger.Error(ex, "Error saving updated scoring information for artifactId {0}", Encoding.UTF8.GetString(natsargs.Message.Data));
                }
            };

            // The simple way to create an asynchronous subscriber
            // is to simply pass the event in.  Messages will start
            // arriving immediately.
            logger.Info("setting up the openSTIG save new subscriptions");
            IAsyncSubscription asyncNew = c.SubscribeAsync("openstig.save.new", newChecklistScore);
            logger.Info("setting up the openSTIG save update subscriptions");
            IAsyncSubscription asyncUpdate = c.SubscribeAsync("openstig.save.update", updateChecklistScore);
            logger.Info("openSTIG subscriptions set successfully!");
        }
        private static ObjectId GetInternalId(string id)
        {
            ObjectId internalId;
            if (!ObjectId.TryParse(id, out internalId))
                internalId = ObjectId.Empty;
            return internalId;
        }
    }
}
