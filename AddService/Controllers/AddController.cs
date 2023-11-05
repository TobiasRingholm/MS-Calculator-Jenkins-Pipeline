using Microsoft.AspNetCore.Mvc;

namespace HelloService.Controllers;

[ApiController]
[Route("[controller]")]
public class AddController : ControllerBase
{
    [HttpGet]
    public string Get(double numberA, double numberB)
    {
        Console.WriteLine(Environment.MachineName);
        var result = numberA + numberB;
        return result.ToString();
    }
}