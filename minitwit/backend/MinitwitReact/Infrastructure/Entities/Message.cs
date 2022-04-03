namespace MinitwitReact.Infrastructure.Entities
{
    public class Message
    {
        public long MessageId { get; set; }
        public User? Author { get; init; }
        public long AuthorId { get; init; }
        public string? Text { get; set; }
        public long PubDate { get; init; }
        public long? Flagged { get; set; }
    }
}
