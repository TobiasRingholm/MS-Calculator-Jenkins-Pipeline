using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Monitoring;
using SharedModel;
using FeatureHubSDK;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polly;

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
            var retryPolicy = Policy.Handle<Exception>()
                .RetryAsync(3);

            var circuitBreakerPolicy = Policy.Handle<Exception>()
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));

            var policy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);

            result = (double)await policy.ExecuteAsync(async () =>
            {
                return await FetchAddAsync(numberA, numberB);
            });
        }
        else if (calculation == "Subtract")
        {
            var retryPolicy = Policy.Handle<Exception>()
                .RetryAsync(3);

            var circuitBreakerPolicy = Policy.Handle<Exception>()
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));

            var policy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);

            result = (double)await policy.ExecuteAsync(async () =>
            {
                return await FetchSubtractAsync(numberA, numberB);
            });
        }
        else if (calculation == "Multiply" && multiplyFeature)
        {
            var retryPolicy = Policy.Handle<Exception>()
                .RetryAsync(3);

            var circuitBreakerPolicy = Policy.Handle<Exception>()
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));

            var policy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);

            result = (double)await policy.ExecuteAsync(async () =>
            {
                return await FetchMultiplyAsync(numberA, numberB);
            });
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
        var config = new EdgeFeatureHubConfig("http://featurehub:8085", "fe9d55a3-b1cc-4db0-8c37-f7d769691a2e/GPMph3M7fTF6YNs0rkyiGp7USG0Q878oRSYwuv2u");
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
