namespace MinitwitReact.Infrastructure.Entities
{
    public class Follower
    {
        // Following 
        public long? WhoId { get; init; }
        // Following me
        public long? WhomId { get; init; }
    }
}
