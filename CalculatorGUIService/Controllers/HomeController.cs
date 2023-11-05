using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CalculatorGUIService.Models;
using RestSharp;

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
        ViewData["result"] = FetchCalculation(parseExpression.value1, parseExpression.value2, parseExpression.calculate);
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
        var task = RestClient.GetAsync<double>(new RestRequest($"Calculation?numberA={numberA}&numberB={numberB}&calculation={calculation}"));
        return task.Result;
    }
}