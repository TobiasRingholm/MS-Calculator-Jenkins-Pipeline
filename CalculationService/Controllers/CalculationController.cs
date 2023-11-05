using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace CalculationService.Controllers;

[ApiController]
[Route("[controller]")]
public class CalculationController : ControllerBase
{
    private static RestClient AddRestClient = new RestClient("http://add-service/");
    private static RestClient SubtractRestClient = new RestClient("http://subtract-service/");
    
    public string Get()
    {
        var add = FetchAdd(); 
        var subtract = FetchSubtract(); 
        
        Console.WriteLine(Environment.MachineName);
        Console.WriteLine($"{add}, {subtract}!");
        return add + " " + subtract + "!";
    }
    
    private static string? FetchSubtract()
    {
        double numbera = 2.2;
        double numberb = 4.2;
        var task = SubtractRestClient.GetAsync<string>(new RestRequest($"Subtract?numberA={numbera}&numberB={numberb}"));
        return task.Result;
    }
    
    private static string? FetchAdd()
    {
        double numbera = 2.2;
        double numberb = 2.2;
        var task = AddRestClient.GetAsync<string>(new RestRequest($"Add?numberA={numbera}&numberB={numberb}"));
        return task.Result;
    }
}