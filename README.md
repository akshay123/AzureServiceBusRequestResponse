AzureServiceBusRequestResponse
==============================

A sample demo application that shows how to use Azure Service Bus using Request
Response AMQP 1.0 recommended approach. The application using SessionId and
ReplyToSessionId to match request and response.

Dotnet core
-----------

This sample uses dotnet core 2.2. and is motivated by .NET Full framework sample
here
https://github.com/Azure/azure-service-bus/tree/master/samples/DotNet/Microsoft.Azure.ServiceBus/QueuesRequestResponse

To do
-----

Things to do before you make it work; 
1. Make sure you create Azure Service bus (ASB). 
2. Create two queues in the same ASB Namespace 
a. requestqueue
b. responsequeue
3. Go to Utility Project and enter your Service Bus connection string. 

a. requestqueue

b. responsequeue

3. Go to Utility Project and enter your Service Bus connection string.

Running
-------

1.  Run the dotnet ReqRespServer.dll to start the server.

2.  Run the dotnet ReqRespClient.dll to run the client.
