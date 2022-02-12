using Microsoft.AspNetCore.Mvc;
namespace MinitwitReact.Controllers;

[ApiController]
[Route("[controller]")]
public class MinitwitController : ControllerBase
{
    private readonly ILogger<MinitwitController> _logger;
    private readonly IMinitwit _minitwit;

    public MinitwitController(ILogger<MinitwitController> logger, IMinitwit minitwit)
    {
        _logger = logger;
        _minitwit = minitwit;
    }

    [HttpGet]
    public void Get()
    {
        _minitwit.QueryDb();
    }
}