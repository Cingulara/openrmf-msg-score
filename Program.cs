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
            IConnection c = cf.CreateConnection();

            // Setup an event handler to process incoming messages.
            // An anonymous delegate function is used for brevity.
            EventHandler<MsgHandlerEventArgs> newChecklist = (sender, natsargs) =>
            {
                // print the message
                Console.WriteLine(natsargs.Message.Subject);
                Console.WriteLine(Encoding.UTF8.GetString(natsargs.Message.Data));
                var result = WebClient.GetChecklistXML(Encoding.UTF8.GetString(natsargs.Message.Data));

                // call the URL above with the proper /download/{id} where the ID is the message data

                //  art.CHECKLIST = ChecklistLoader.LoadChecklist(art.rawChecklist);
                // httpclient to call http://localhost:8084/download/1276172a-771a-48cc-b056-2d2fe9889746 to get an XML string


                // Here are some of the accessible properties from
                // the message:
                // args.Message.Data;
                // args.Message.Reply;
                // args.Message.Subject;
                // args.Message.ArrivalSubcription.Subject;
                // args.Message.ArrivalSubcription.QueuedMessageCount;
                // args.Message.ArrivalSubcription.Queue;

                // Unsubscribing from within the delegate function is supported.
                // natsargs.Message.ArrivalSubcription.Unsubscribe();

            };
            EventHandler<MsgHandlerEventArgs> update = (sender, natsargs) =>
            {
                // print the message
                Console.WriteLine(natsargs.Message.Subject);
                Console.WriteLine(Encoding.UTF8.GetString(natsargs.Message.Data));
                // Here are some of the accessible properties from
                // the message:
                // args.Message.Data;
                // args.Message.Reply;
                // args.Message.Subject;
                // args.Message.ArrivalSubcription.Subject;
                // args.Message.ArrivalSubcription.QueuedMessageCount;
                // args.Message.ArrivalSubcription.Queue;

                // Unsubscribing from within the delegate function is supported.
                //natsargs.Message.ArrivalSubcription.Unsubscribe();
            };

            // The simple way to create an asynchronous subscriber
            // is to simply pass the event in.  Messages will start
            // arriving immediately.
            IAsyncSubscription asyncNew = c.SubscribeAsync("openstig.save.new", newChecklist);
            IAsyncSubscription asyncUpdate = c.SubscribeAsync("openstig.save.update", update);
        }
    }
}
