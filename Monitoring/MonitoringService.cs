﻿using System.Diagnostics;
using System.Reflection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;

namespace Monitoring;

public static class MonitoringService
{
    public static readonly string ServiceName = Assembly.GetCallingAssembly().GetName().Name ?? "Unknown";
    public static TracerProvider TracerProvider;
    public static readonly ActivitySource ActivitySource = new(ServiceName, "1.0.0");
    public static ILogger Log => Serilog.Log.Logger;

    public static void Initialize()
    {
        // Denne tjekker, om TracerProvider allerede er initialiseret
        if (TracerProvider != null) return;

        // OpenTelemetry
        TracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddZipkinExporter(options => options.Endpoint = new Uri("http://zipkin:9411/api/v2/spans"))
            .AddConsoleExporter()
            .AddSource(ActivitySource.Name)
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName: ActivitySource.Name))
            .Build()!;
        
        // Configure logging
        Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Seq("http://seq:5341")
            .WriteTo.Console()
            .Enrich.WithSpan()
            .CreateLogger();
    }
}