namespace MinitwitReact.Core.DTO;

public record MessageDto (long MessageId, string Author, string Text, long PubDate);

public record MessageDetailsDto(long MessageId, string Author, string Text, long PubDate, long Flagged);

public record MessageCreateDto
{
    public string? Author { get; set; }
    public string Text { get; set; } = null!;
    public long PubDate { get; set; }
    public long? Flagged { get; set; }
}

public record MessageUpdateDto : MessageCreateDto
{
    public long MessageId { get; set; }
}