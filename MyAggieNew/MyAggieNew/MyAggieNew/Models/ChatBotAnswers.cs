using System;
using System.Collections.Generic;

namespace MyAggieNew
{
    public class ChatBotAnswers
    {
        public List<Answer> answers { get; set; }
        public object dwqebugInfo { get; set; }
    }

    public class ContextObject
    {
        public bool isContextOnly { get; set; }
        public List<object> prompts { get; set; }
    }

    public class Answer
    {
        public List<string> questions { get; set; }
        public string answer { get; set; }
        public double score { get; set; }
        public int id { get; set; }
        public string source { get; set; }
        public List<object> metadata { get; set; }
        public ContextObject context { get; set; }
    }
}