using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CalculatorGUIService.Models;
using RestSharp;
using Monitoring;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using Serilog;
using SharedModel;

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
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered load history method");

        var calculationsList = FetchAllCalculations();
        var displayList = calculationsList.Select(calc => 
            $"ID: {calc.ID}, Value1: {calc.Value1}, Value2: {calc.Value2}, Result: {calc.Result}, Operator: {calc.MathOperator}"
        ).ToList();

        var viewModel = new CalculationViewModel
        {
            CalculationsDisplay = displayList
        };

        return View(viewModel);
    }

    
    [HttpPost]
    public IActionResult Privacy(ParseExpression parseExpression)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered load history method");
        using (MonitoringService.Log.Here().BeginTimedOperation("Running load history from db in gui"))
        {
            var list = FetchAllCalculations();
            var allCalculations = FetchAllCalculations();
            if (allCalculations != null)
                foreach (var calculation in allCalculations)
                {
                    Console.WriteLine(
                        $"ID: {calculation.ID}, Value1: {calculation.Value1}, Value2: {calculation.Value2}, Result: {calculation.Result}, Operator: {calculation.MathOperator}");
                }
            ViewData["history"] = list;
        }
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
    
    private static List<Calculation>? FetchAllCalculations()
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered FetchAllCalculations method");
        var task = RestClient.PostAsync<List<Calculation>>(new RestRequest($"Calculation"));
        
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