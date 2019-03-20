using System;
using System.Diagnostics;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System.Threading.Tasks;
using Utility;



namespace ReqRespClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client Console");

            // Use this is for interactive mode. 
            SendRecieveAsync().Wait();

            // Use this for trigger based run. 
            //SendNow().Wait();
        }

        // Test Queues on Azure. 
        static MessageSender mSender = new MessageSender(AccountDetails.ConnectionString, AccountDetails.RequestQueueName);
        static SessionClient mSessionRecvr = new SessionClient(AccountDetails.ConnectionString, AccountDetails.ResponseQueueName, ReceiveMode.ReceiveAndDelete);
        static MessageSender mSimple = new MessageSender(AccountDetails.ConnectionString, "simple");


        private static async Task SendNow()
        {
            // Test data to send in trigger based run. 
            string text = "hello";
            string text1 = "hello1";
            string text2 = "hello2";
            string text3 = "hello3";

            await Send(text);
            await Send(text1);
            await Send(text2);
            await Send(text3);

        }

        private static async Task SendRecieveAsync()
        {
            while (true)
            {
                Console.WriteLine("------------------");
                Console.WriteLine("Enter text:");
                string text = Console.ReadLine();
                await Send(text);
            }
        }

        private static async Task Send(string text)
        {
            var stopwatch = new Stopwatch();

            #region ["Send A Simple Msg"]
            Message simpleMessage = new Message
            {
                Body = StringSupport.GetBytes(text)
            };

            stopwatch.Start();
            // Send the message on the request queue.
            await mSimple.SendAsync(simpleMessage);
            stopwatch.Stop();

            Console.WriteLine("---");
            Console.WriteLine("Simple Msg: {0} ms.", stopwatch.ElapsedMilliseconds);
      
   
            #endregion


            #region ["Send Req-Resp Msg"]
            // Create a session identifyer for the response message
            string responseSessionId = Guid.NewGuid().ToString();

            // Create a message using text as the body.
            Message requestMessage = new Message
            {
                SessionId = responseSessionId,
                Body = StringSupport.GetBytes(text),
                ReplyToSessionId = responseSessionId
            };
            Console.WriteLine("---");
            Console.WriteLine("Sending request to RequestQueue for processing...");

            stopwatch.Start();
                // Send the message on the request queue.
            await mSender.SendAsync(requestMessage);
            
            // Accept a session message.
            var responseSession = await mSessionRecvr.AcceptMessageSessionAsync(responseSessionId);
            Message responseMessage = await responseSession.ReceiveAsync();
            stopwatch.Stop();

            await responseSession.CloseAsync();
            #endregion

            // Deserialise the message body to echoText.
            string echoText = StringSupport.GetString(responseMessage.Body);


            Console.WriteLine("Response rcvd from Server.");
            Console.WriteLine(echoText);
            Console.WriteLine("Req-Resp (Total) Time: {0} ms.", stopwatch.ElapsedMilliseconds);
       
        }
    }
}

