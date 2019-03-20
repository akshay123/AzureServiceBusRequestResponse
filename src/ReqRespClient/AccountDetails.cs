using System;
namespace ReqRespClient
{
    public static class AccountDetails
    {

        public static string ConnectionString = "Endpoint=sb://asb-one.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=MAe8EjIw15GMyxkbLOec4SXN0mYzd/gzjEeK8iwMln8=";
        public static string RequestQueueName = "requestqueue";
        public static string ResponseQueueName = "responsequeue";
    }
}
