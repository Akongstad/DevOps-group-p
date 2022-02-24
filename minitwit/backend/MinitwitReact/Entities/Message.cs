using System;
using System.Collections.Generic;

namespace MinitwitReact.Entities
{
    public class Message
    {
        public long MessageId { get; set; }
        public User? Author { get; set; }
        public long AuthorId { get; set; }
        public string? Text { get; set; }
        public long PubDate { get; set; }
        public long? Flagged { get; set; }
    }
}
