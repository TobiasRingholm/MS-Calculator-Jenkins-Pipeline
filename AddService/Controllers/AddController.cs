using Microsoft.AspNetCore.Mvc;
using Monitoring;

namespace HelloService.Controllers;

[ApiController]
[Route("[controller]")]
public class AddController : ControllerBase
{
    [HttpGet]
    public string Get(double numberA, double numberB)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered Add method");
        var result = numberA + numberB;
        return result.ToString();
    }
}