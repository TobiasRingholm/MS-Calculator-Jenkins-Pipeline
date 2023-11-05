using Microsoft.AspNetCore.Mvc;

namespace WorldService.Controllers;

[ApiController]
[Route("[controller]")]
public class SubtractController : ControllerBase
{
    [HttpGet]
    public string Get(double numberA, double numberB)
    {
        Console.WriteLine(Environment.MachineName);
        var result = numberA - numberB;
        return result.ToString();
    }
}