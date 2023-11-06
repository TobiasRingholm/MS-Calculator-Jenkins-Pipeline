using Microsoft.AspNetCore.Mvc;
using Monitoring;

namespace WorldService.Controllers;

[ApiController]
[Route("[controller]")]
public class SubtractController : ControllerBase
{
    [HttpGet]
    public string Get(double numberA, double numberB)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered Add method");
        var result = numberA - numberB;
        return result.ToString();
    }
}