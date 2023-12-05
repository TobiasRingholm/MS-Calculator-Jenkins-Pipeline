using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Monitoring;
using SharedModel;
using FeatureHubSDK;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        using var activity = MonitoringService.ActivitySource.StartActivity("CalculationController.Get");
        MonitoringService.Log.Here().Debug("Entered Calculation method with the {calculation} operator", calculation);
        ConfigureFeatureHubLogging();

        var config = await ConfigureFeatureHub();
        var multiplyFeature = config[$"multiService"].IsEnabled;

        double result = 0;
        if (calculation == "Add")
        {
            result = (double)await FetchAddAsync(numberA, numberB);
        }
        else if (calculation == "Subtract")
        {
            result = (double)await FetchSubtractAsync(numberA, numberB);
        }
        else if (calculation == "Multiply" && multiplyFeature)
        {
            result = (double)await FetchMultiplyAsync(numberA, numberB);
        }

        if (result != 0)
        {
            await PostCalculationAsync(numberA, numberB, result, calculation);
        }

        return result;
    }

    [HttpPost]
    public async Task<List<Calculation>?> Post()
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("CalculationController.Post");
        MonitoringService.Log.Here().Debug("Entered method to call History service to get db data");
        return await FetchCalculationHistoryAsync();
    }

    private async Task<double?> FetchSubtractAsync(double numberA, double numberB)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("FetchSubtract");
        MonitoringService.Log.Here().Debug("Entered FetchSubtract method");
        var request = new RestRequest($"Subtract?numberA={numberA}&numberB={numberB}");
        InjectTraceContext(request);
        return await SubtractRestClient.GetAsync<double>(request);
    }

    private async Task<double?> FetchAddAsync(double numberA, double numberB)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("FetchAdd");
        MonitoringService.Log.Here().Debug("Entered FetchAdd method");
        var request = new RestRequest($"Add?numberA={numberA}&numberB={numberB}");
        InjectTraceContext(request);
        return await AddRestClient.GetAsync<double>(request);
    }

    private async Task<double?> FetchMultiplyAsync(double numberA, double numberB)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("FetchMultiply");
        MonitoringService.Log.Here().Debug("Entered FetchMultiply method");
        var request = new RestRequest($"Multiply?numberA={numberA}&numberB={numberB}");
        InjectTraceContext(request);
        return await MultiplyRestClient.GetAsync<double>(request);
    }

    private static async Task<List<Calculation>?> FetchCalculationHistoryAsync()
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("FetchCalculationHistory");
        MonitoringService.Log.Here().Debug("Entered FetchCalculationHistory method");
        var request = new RestRequest($"CalculationHistory");
        InjectTraceContext(request);
        return await HistoryRestClient.GetAsync<List<Calculation>>(request);
    }

    private static async Task PostCalculationAsync(double numberA, double numberB, double result, string mathOperator)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("PostCalculation");
        MonitoringService.Log.Here().Debug("Entered PostCalculation method");
        var request = new RestRequest($"CalculationHistory?numberA={numberA}&numberB={numberB}&result={result}&mathOperator={mathOperator}");
        InjectTraceContext(request);
        await HistoryRestClient.PostAsync(request);
    }

    private static void InjectTraceContext(RestRequest request)
    {
        if (Activity.Current != null)
        {
            request.AddHeader("traceparent", Activity.Current.Id);
        }
    }

    private async Task<IClientContext> ConfigureFeatureHub()
    {
        var config = new EdgeFeatureHubConfig("http://featurehub:8085", "6b67b572-afc1-42f5-a58e-58bbe03b7765/z2YCy3Vw9tOW3VsqeSO95HbXEmlkrohFGjGB086o");
        var fh = await config.NewContext().Build();
        return fh;
    }

    private void ConfigureFeatureHubLogging()
    {
        FeatureLogging.DebugLogger += (sender, s) => MonitoringService.Log.Here().Debug(s);
        FeatureLogging.TraceLogger += (sender, s) => MonitoringService.Log.Here().Verbose(s);
        FeatureLogging.InfoLogger += (sender, s) => MonitoringService.Log.Here().Information(s);
        FeatureLogging.ErrorLogger += (sender, s) => MonitoringService.Log.Here().Error(s);
    }
}
