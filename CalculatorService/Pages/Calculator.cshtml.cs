using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestSharp;
using System.Text.RegularExpressions;

namespace CalculatorService.Pages;

public class Calculator : PageModel
{
    private static readonly RestClient RestClient = new RestClient("http://calculation-service/");
    
    private static string? FetchCalculation()
    {
        var task = RestClient.GetAsync<string>(new RestRequest("Calculation"));
        return task.Result;
    }
}
