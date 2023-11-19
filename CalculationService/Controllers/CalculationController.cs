using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Monitoring;
using SharedModel;
using FeatureHubSDK;

namespace CalculationService.Controllers;

[ApiController]
[Route("[controller]")]
public class CalculationController : ControllerBase
{
    private static RestClient AddRestClient = new RestClient("http://add-service/");
    private static RestClient SubtractRestClient = new RestClient("http://subtract-service/");
    private static RestClient MultiplyRestClient = new RestClient("http://multiply-service/");
    private static RestClient HistoryRestClient = new RestClient("http://calculation-history-service/");
    
    [HttpGet]
    public async Task<double> Get(double numberA, double numberB, string calculation)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered Calculation method with the {calculation} operator",calculation);
        FeatureLogging.DebugLogger += (sender, s) => Console.WriteLine("DEBUG: " + s);
        FeatureLogging.TraceLogger += (sender, s) => Console.WriteLine("TRACE: " + s);
        FeatureLogging.InfoLogger += (sender, s) => Console.WriteLine("INFO: " + s);
        FeatureLogging.ErrorLogger += (sender, s) => Console.WriteLine("Error: " + s);
        var config = new EdgeFeatureHubConfig("http://featurehub:8085", "21113396-0633-4bf2-8fcb-cf98fc707cb5/4vnPcTKNDJGBBS3ZH1qGYUl6BtIMoa2pZmuQPu7C");
        var fh = await config.NewContext().Build();
        var multiplyFeature = fh["multiService"].IsEnabled;
        if (calculation == "Add")
        {
            var result = (double)FetchAdd(numberA, numberB);
            PostCalculation(numberA, numberB, result, calculation);
            return result;
        }
        else if (calculation == "Subtract")
        {
            var result = (double)FetchSubtract(numberA, numberB);
            PostCalculation(numberA, numberB, result, calculation);
            return result;
        }
        else if (calculation == "Multiply" && multiplyFeature)
        {
            var result = (double)FetchMultiply(numberA, numberB);
            PostCalculation(numberA, numberB, result, calculation);
            return result;
        }
        else
        {
            return 0;
        }
    }
    [HttpPost]
    public List<Calculation>? Post()
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered method to call History service to get db data");
        return FetchCalculationHistory();
    }
    
    private double? FetchSubtract(double numberA, double numberB)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered FetchSubtract method");
        var task = SubtractRestClient.GetAsync<double>(new RestRequest($"Subtract?numberA={numberA}&numberB={numberB}"));
        return task.Result;
    }
    
    private double? FetchAdd(double numberA, double numberB)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered FetchAdd method");
        var task = AddRestClient.GetAsync<double>(new RestRequest($"Add?numberA={numberA}&numberB={numberB}"));
        return task.Result;
    }
    
    private double? FetchMultiply(double numberA, double numberB)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered FetchAdd method");
        var task = MultiplyRestClient.GetAsync<double>(new RestRequest($"Multiply?numberA={numberA}&numberB={numberB}"));
        return task.Result;
    }
    
    private static List<Calculation>? FetchCalculationHistory()
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered FetchCalculationHistory method");
        var task = HistoryRestClient.GetAsync<List<Calculation>>(new RestRequest($"CalculationHistory"));
        return task.Result;
    }
    
    private static void PostCalculation(double numberA, double numberB, double result, string mathOperator)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered PostCalculation method");
        HistoryRestClient.PostAsync(new RestRequest($"CalculationHistory?numberA={numberA}&numberB={numberB}&result={result}&mathOperator={mathOperator}"));
    }
}