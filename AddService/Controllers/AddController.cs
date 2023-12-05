using Microsoft.AspNetCore.Mvc;
using Monitoring;
using OpenTelemetry.Trace;

namespace HelloService.Controllers;

[ApiController]
[Route("[controller]")]
public class AddController : ControllerBase
{
    /*** START OF IMPORTANT CONFIGURATION ***/
    private readonly Tracer _tracer;
    public AddController(Tracer tracer)
    {
        _tracer = tracer;
    }
    /*** END OF IMPORTANT CONFIGURATION ***/
    
    [HttpGet]
    public string Get(double numberA, double numberB)
    {
        using var activity = _tracer.StartActiveSpan("GET");
        MonitoringService.Log.Here().Debug("Entered Add method");
        var result = numberA + numberB;
        return result.ToString();
    }
}