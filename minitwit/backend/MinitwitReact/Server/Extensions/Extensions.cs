namespace MinitwitReact.Server.Extensions;

public static class Extensions
{
    public static IActionResult ToActionResult(this Status status) => status switch
    {
        Status.Updated => new NoContentResult(),
        Status.Deleted => new NoContentResult(),
        Status.NotFound => new NotFoundResult(),
        Status.Conflict => new ConflictResult(),
        Status.BadRequest => new BadRequestResult(),
        Status.Created => new NoContentResult(),
        _ => throw new NotSupportedException($"{status} not recognised")
    };
}