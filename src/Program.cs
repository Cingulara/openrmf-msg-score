using System;
using NATS.Client;
using System.Text;
using System.IO;
using System.IO.Compression;
using NLog;
using NLog.Config;
using openrmf_msg_score.Models;
using openrmf_msg_score.Classes;
using Newtonsoft.Json;

using MongoDB.Bson;

namespace openrmf_msg_score
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.Configuration = new XmlLoggingConfiguration($"{AppContext.BaseDirectory}nlog.config");

            var logger = LogManager.GetLogger("openrmf-msg-score");

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
                    //Artifact checklist = WebClient.GetChecklistAsync(Encoding.UTF8.GetString(natsargs.Message.Data)).GetAwaiter().GetResult();
                    Artifact checklist = GetChecklist(c, Encoding.UTF8.GetString(natsargs.Message.Data));
                    if (checklist != null && checklist.CHECKLIST != null) {
                        Score score = ScoringEngine.ScoreChecklistString(checklist.rawChecklist);
                        score.system = checklist.system;
                        score.stigType = checklist.stigType;
                        score.stigRelease = checklist.stigRelease;
                        score.artifactId = GetInternalId(Encoding.UTF8.GetString(natsargs.Message.Data));
                        score.hostName = checklist.hostName;
                        score.created = DateTime.Now;
                        logger.Info("Saving new score for artifactId {0}", score.artifactId.ToString());
                        score.SaveScore();
                        logger.Info("Score successfully saved for artifactId {0}", score.artifactId.ToString());
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
                    //Artifact checklist = WebClient.GetChecklistAsync(Encoding.UTF8.GetString(natsargs.Message.Data)).GetAwaiter().GetResult();
                    Artifact checklist = GetChecklist(c, Encoding.UTF8.GetString(natsargs.Message.Data));
                    if (checklist != null && checklist.CHECKLIST != null) {
                        Score score = ScoringEngine.ScoreChecklistString(checklist.rawChecklist);   
                        score.system = checklist.system;
                        score.stigType = checklist.stigType;
                        score.stigRelease = checklist.stigRelease;
                        score.created = checklist.created;
                        score.artifactId = GetInternalId(Encoding.UTF8.GetString(natsargs.Message.Data));
                        score.hostName = checklist.hostName;
                        score.updatedOn = DateTime.Now;
                        logger.Info("Saving updated score for artifactId {0}", score.artifactId.ToString());
                        score.UpdateScore();
                        logger.Info("Score successfully updated for artifactId {0}", score.artifactId.ToString());
                    }
                }
                catch (Exception ex) {
                    // log it here
                    logger.Error(ex, "Error saving updated scoring information for artifactId {0}", Encoding.UTF8.GetString(natsargs.Message.Data));
                }
            };

            EventHandler<MsgHandlerEventArgs> deleteChecklistScore = (sender, natsargs) =>
            {
                try {
                    // print the message
                    Console.WriteLine(natsargs.Message.Subject);
                    Console.WriteLine(Encoding.UTF8.GetString(natsargs.Message.Data));
                    Score score = new Score();
                    score.artifactId = GetInternalId(Encoding.UTF8.GetString(natsargs.Message.Data));
                    logger.Info("Deleting score for artifactId {0}", score.artifactId.ToString());
                    score.RemoveScore();
                    logger.Info("Score deleted successfully for artifactId {0}", score.artifactId.ToString());
                }
                catch (Exception ex) {
                    // log it here
                    logger.Error(ex, "Error deleting scoring information for artifactId {0}", Encoding.UTF8.GetString(natsargs.Message.Data));
                }
            };

            // The simple way to create an asynchronous subscriber
            // is to simply pass the event in.  Messages will start
            // arriving immediately.
            logger.Info("setting up the openRMF new score subscriptions");
            IAsyncSubscription asyncNew = c.SubscribeAsync("openrmf.save.new", newChecklistScore);
            logger.Info("setting up the openRMF update score subscriptions");
            IAsyncSubscription asyncUpdate = c.SubscribeAsync("openrmf.save.update", updateChecklistScore);
            logger.Info("setting up the openRMF delete score subscriptions");
            IAsyncSubscription asyncDelete = c.SubscribeAsync("openrmf.delete", deleteChecklistScore);
            logger.Info("openRMF subscriptions set successfully!");
        }

        // make the string an internal ID for MongoDB
        private static ObjectId GetInternalId(string id)
        {
            ObjectId internalId;
            if (!ObjectId.TryParse(id, out internalId))
                internalId = ObjectId.Empty;
            return internalId;
        }

        private static Artifact GetChecklist(IConnection conn, string id){
            try {
                Artifact art = new Artifact();
                Msg reply = conn.Request("openrmf.checklist.read", Encoding.UTF8.GetBytes(id), 30000); // publish to get this Artifact checklist back via ID
                // save the reply and get back the checklist to score
                if (reply != null) {
                    art = JsonConvert.DeserializeObject<Artifact>(DecompressString(Encoding.UTF8.GetString(reply.Data)));
                    return art;
                }
                return art;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns></returns>
        public static string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
    }
}
