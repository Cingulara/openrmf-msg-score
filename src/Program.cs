// Copyright (c) Cingulara LLC 2019 and Tutela LLC 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using System;
using NATS.Client;
using System.Text;
using NLog;
using NLog.Config;
using openrmf_msg_score.Models;
using openrmf_msg_score.Classes;
using openrmf_msg_score.Data;
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
            // add the options for the server, reconnecting, and the handler events
            Options opts = ConnectionFactory.GetDefaultOptions();
            opts.MaxReconnect = -1;
            opts.ReconnectWait = 2000;
            opts.Name = "openrmf-msg-score";
            opts.Url = Environment.GetEnvironmentVariable("NATSSERVERURL");
            opts.AsyncErrorEventHandler += (sender, events) =>
            {
                logger.Info("NATS client error. Server: {0}. Message: {1}. Subject: {2}", events.Conn.ConnectedUrl, events.Error, events.Subscription.Subject);
            };

            opts.ServerDiscoveredEventHandler += (sender, events) =>
            {
                logger.Info("A new server has joined the cluster: {0}", events.Conn.DiscoveredServers);
            };

            opts.ClosedEventHandler += (sender, events) =>
            {
                logger.Info("Connection Closed: {0}", events.Conn.ConnectedUrl);
            };

            opts.ReconnectedEventHandler += (sender, events) =>
            {
                logger.Info("Connection Reconnected: {0}", events.Conn.ConnectedUrl);
            };

            opts.DisconnectedEventHandler += (sender, events) =>
            {
                logger.Info("Connection Disconnected: {0}", events.Conn.ConnectedUrl);
            };
            
            // Creates a live connection to the NATS Server with the above options
            IConnection c = cf.CreateConnection(opts);

            // Setup a new Score record based on a new checklist uploaded
            // This is called from the Upload API to say "hey I have a new checklist, score it"
            EventHandler<MsgHandlerEventArgs> newChecklistScore = (sender, natsargs) =>
            {
                try {
                    // print the message
                    logger.Info("New NATS subject: {0}", natsargs.Message.Subject);
                    logger.Info("New NATS data: {0}",Encoding.UTF8.GetString(natsargs.Message.Data));
                    Artifact checklist = GetChecklist(c, Encoding.UTF8.GetString(natsargs.Message.Data));
                    if (checklist != null && checklist.CHECKLIST != null) {
                        Score score = ScoringEngine.ScoreChecklistString(checklist.rawChecklist);
                        score.systemGroupId = checklist.systemGroupId;
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

            // Setup an updated Score record based on an updated checklist uploaded
            // This is called from the Upload API to say "hey I have an updated checklist, you may want to update your scoring"
            EventHandler<MsgHandlerEventArgs> updateChecklistScore = (sender, natsargs) =>
            {
                try {
                    // print the message
                    Console.WriteLine(natsargs.Message.Subject);
                    Console.WriteLine(Encoding.UTF8.GetString(natsargs.Message.Data));
                    Artifact checklist = GetChecklist(c, Encoding.UTF8.GetString(natsargs.Message.Data));
                    if (checklist != null && checklist.CHECKLIST != null) {
                        Score score = ScoringEngine.ScoreChecklistString(checklist.rawChecklist);   
                        score.systemGroupId = checklist.systemGroupId;
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

            // Delete the score record and clean up the data
            // This is called from the Save API to say "hey I just deleted a checklist, clean up the scoring record"
            EventHandler<MsgHandlerEventArgs> deleteChecklistScore = (sender, natsargs) =>
            {
                try {
                    // print the message
                    logger.Info(natsargs.Message.Subject);
                    logger.Info(Encoding.UTF8.GetString(natsargs.Message.Data));
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

            // Respond to a request to read the score
            // Called from the Read API when someone downloads a system checklist listing with the scores in it
            EventHandler<MsgHandlerEventArgs> readChecklistScore = (sender, natsargs) =>
            {
                try {
                    logger.Info("OpenRMF Score Client: {0}", natsargs.Message.Subject);
                    logger.Info("Score for Artifact: {0}",Encoding.UTF8.GetString(natsargs.Message.Data));
                    if (!string.IsNullOrEmpty(Encoding.UTF8.GetString(natsargs.Message.Data))) {
                        Score score = new Score();
                        Settings s = new Settings();
                        s.ConnectionString = Environment.GetEnvironmentVariable("MONGODBCONNECTION");
                        s.Database = Environment.GetEnvironmentVariable("MONGODB");
                        ScoreRepository _scoreRepo = new ScoreRepository(s);
                        logger.Info("Retrieving Score for artifactId {0}", score.artifactId.ToString());
                        score = _scoreRepo.GetScorebyArtifact(Encoding.UTF8.GetString(natsargs.Message.Data)).GetAwaiter().GetResult();
                        string msg = "";
                        if (score != null) {
                            // put into a JSON string
                            msg = JsonConvert.SerializeObject(score);
                        } 
                        else {
                            msg = JsonConvert.SerializeObject(new Score());
                        }
                        // send the reply back to the calling request
                        c.Publish(natsargs.Message.Reply, Encoding.UTF8.GetBytes(Compression.CompressString(msg)));
                        // flush the line
                        c.Flush();
                        logger.Info("Score successfully sent back for artifactId {0}", score.artifactId.ToString());
                    }
                }
                catch (Exception ex) {
                    // log it here
                    logger.Error(ex, "Error reading scoring information for artifactId {0}", Encoding.UTF8.GetString(natsargs.Message.Data));
                }
            };

            // The simple way to create an asynchronous subscriber
            // is to simply pass the event in.  Messages will start
            // arriving immediately.
            logger.Info("setting up the openRMF new score subscriptions");
            IAsyncSubscription asyncNew = c.SubscribeAsync("openrmf.checklist.save.new", newChecklistScore);
            logger.Info("setting up the openRMF update score subscriptions");
            IAsyncSubscription asyncUpdate = c.SubscribeAsync("openrmf.checklist.save.update", updateChecklistScore);
            logger.Info("setting up the openRMF delete score subscriptions");
            IAsyncSubscription asyncDelete = c.SubscribeAsync("openrmf.checklist.delete", deleteChecklistScore);
            logger.Info("openRMF subscriptions set successfully!");
            logger.Info("setting up the openRMF score read subscription");
            IAsyncSubscription asyncRead = c.SubscribeAsync("openrmf.score.read", readChecklistScore);
        }

        /// <summary>
        /// Turn the string ID into the ObjectId of a database key record
        /// </summary>
        /// <param name="id">The string id for the database to turn into an object for use</param>
        /// <returns>An objectID for the string ID to use in the database</returns>
        private static ObjectId GetInternalId(string id)
        {
            ObjectId internalId;
            if (!ObjectId.TryParse(id, out internalId))
                internalId = ObjectId.Empty;
            return internalId;
        }

        /// <summary>
        /// Return a checklist record based on the ID requested. Uses a request/reply 
        /// method to get a checklist and then score it.
        /// </summary>
        /// <param name="conn">The database connection</param>
        /// <param name="id">The id of the checklist record to return</param>
        /// <returns>A checklist record, if found</returns>
        private static Artifact GetChecklist(IConnection conn, string id){
            try {
                Artifact art = new Artifact();
                Msg reply = conn.Request("openrmf.checklist.read", Encoding.UTF8.GetBytes(id), 10000); // publish to get this Artifact checklist back via ID
                // save the reply and get back the checklist to score
                if (reply != null) {
                    art = JsonConvert.DeserializeObject<Artifact>(Compression.DecompressString(Encoding.UTF8.GetString(reply.Data)));
                    return art;
                }
                return art;
            }
            catch (Exception ex) {
                Console.WriteLine(string.Format("openrmf-msg-score Error in GetChecklist with Artifact id {0}. Message: {1}",
                    id, ex.Message));
                throw ex;
            }
        }

    }
}
