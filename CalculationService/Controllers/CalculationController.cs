using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Monitoring;

namespace CalculationService.Controllers;

[ApiController]
[Route("[controller]")]
public class CalculationController : ControllerBase
{
    private static RestClient AddRestClient = new RestClient("http://add-service/");
    private static RestClient SubtractRestClient = new RestClient("http://subtract-service/");
    
    public double Get(double numberA, double numberB, string calculation)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered Calculation method with the {calclation} operator",calculation);
        if (calculation == "Add")
        {
            return (double)FetchAdd(numberA, numberB); 
        }
        else
        {
            return (double)FetchSubtract(numberA, numberB);
        }
    }
    
    private static double? FetchSubtract(double numberA, double numberB)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered FetchSubtract method");
        var task = SubtractRestClient.GetAsync<double>(new RestRequest($"Subtract?numberA={numberA}&numberB={numberB}"));
        return task.Result;
    }
    
    private static double? FetchAdd(double numberA, double numberB)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered FetchAdd method");
        var task = AddRestClient.GetAsync<double>(new RestRequest($"Add?numberA={numberA}&numberB={numberB}"));
        return task.Result;
    }
}