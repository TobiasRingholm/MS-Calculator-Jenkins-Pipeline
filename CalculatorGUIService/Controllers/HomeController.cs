using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CalculatorGUIService.Models;
using RestSharp;
using Monitoring;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Serilog;

namespace CalculatorGUIService.Controllers;

public class HomeController : Controller
{
    private static readonly RestClient RestClient = new RestClient("http://calculation-service/");
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }   
    [HttpPost]
    public IActionResult Index(ParseExpression parseExpression)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered Expression-Parser method");
        using (MonitoringService.Log.Here().BeginTimedOperation("Running Expression-Parser")){
        ViewData["result"] = FetchCalculation(parseExpression.value1, parseExpression.value2, parseExpression.calculate);
        }
        return View();
    }   

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    private static double? FetchCalculation(double numberA, double numberB, string calculation)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered FetchCalculation method");
        var task = RestClient.GetAsync<double>(new RestRequest($"Calculation?numberA={numberA}&numberB={numberB}&calculation={calculation}"));
        var activityContext = activity?.Context ?? Activity.Current?.Context ?? default;
        var propagationContext = new PropagationContext(activityContext, Baggage.Current);
        var propagator = new TraceContextPropagator();
        propagator.Inject(propagationContext, task, (r, key, value) =>
        {
            //r.Header(key, value);
        });
        var result = task.Result;
        
        return result;
    }
}