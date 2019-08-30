using System;

namespace MyAggieNew
{
    public class Message
    {
        public string message { get; set; }
        public UserLoginInfo sender;
        public DateTime createdAt { get; set; }
    }
}