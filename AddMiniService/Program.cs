using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Monitoring;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;


var builder = WebApplication.CreateBuilder(args);

var serviceName = Assembly.GetCallingAssembly().GetName().Name;
var serviceVersion = "1.0.0";

builder.Services.AddOpenTelemetry()
    .WithTracing(b =>
    {
        b
            .AddAspNetCoreInstrumentation()
            .AddZipkinExporter(config => config.Endpoint = new Uri("http://zipkin:9411/api/v2/spans"))
            .AddSource(serviceName)
            .ConfigureResource(resource =>
                resource.AddService(
                    serviceName: serviceName,
                    serviceVersion: serviceVersion));
    });


var app = builder.Build();

app.MapGet("/add", (double numberA, double numberB) =>
{
    using var activity = Telemetry.ActivitySource.StartActivity("addition");
    MonitoringService.Log.Here().Debug("Entered Add method");
    return numberA + numberB;
});

app.Run();