using System;
using System.Diagnostics;

using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

using Utility;

namespace ReqRespServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server Console");
            InitializeReceiver();
            Console.ReadLine();
        }


        static QueueClient InitializeReceiver()
        {
            var mSender = new MessageSender(AccountDetails.ConnectionString, AccountDetails.ResponseQueueName);
            var mReciever = new QueueClient(AccountDetails.ConnectionString, AccountDetails.RequestQueueName, ReceiveMode.PeekLock);

            mReciever.RegisterSessionHandler(
               async (session, message, cancellationToken) =>
               {
                   var stopwatch = new Stopwatch();
                   stopwatch.Start();
                   var sessionId = message.ReplyToSessionId;
                   var body = message.Body;

                   var msg = StringSupport.GetString(body);

                   string echoText = "Echo: " + msg;

                   var outMsg = new Message
                   {
                       Body = StringSupport.GetBytes(echoText),
                       SessionId = sessionId
                   };
                   await mSender.SendAsync(outMsg);
                   stopwatch.Stop();

                   await session.CompleteAsync(message.SystemProperties.LockToken);
                   await session.CloseAsync();


                   lock (Console.Out)
                   {
                       Console.ForegroundColor = ConsoleColor.Cyan;
                       Console.WriteLine("----");
                       Console.WriteLine("Message received");

                       Console.WriteLine("SessionId  = {0}", message.SessionId);
                       Console.WriteLine("MessageId  = {0}", message.MessageId);
                       Console.WriteLine("SequenceId = {0}", message.SessionId);
                       Console.WriteLine("Message    = {0}", msg);

                       Console.ResetColor();
                   }
                   Console.WriteLine("Time: {0} ms.", stopwatch.ElapsedMilliseconds);
                   Console.WriteLine();


               },
                new SessionHandlerOptions(LogMessageHandlerException)
                {
                    MessageWaitTimeout = TimeSpan.FromSeconds(5),
                    MaxConcurrentSessions = 1,
                    AutoComplete = false
                });
            return mReciever;
        }

        private static Task LogMessageHandlerException(ExceptionReceivedEventArgs e)
        {
            Console.WriteLine("Exception: \"{0}\" {1}", e.Exception.Message, e.ExceptionReceivedContext.EntityPath);
            return Task.CompletedTask;
        }
    }

}
