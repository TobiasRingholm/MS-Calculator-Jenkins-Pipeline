using Microsoft.AspNetCore.Mvc.RazorPages;
using RestSharp;

namespace CalculatorService.Pages;

public class Calculator : PageModel
{
    private static readonly RestClient RestClient = new RestClient("http://calculation-service/");
    
    public bool hasData = false;
    public string firstName = "";
    public string lastName = "";
    public string message = "";
    
    public void OnGet()
    {
        
    }

    public void OnPost()
    {
        hasData = true;
        firstName = Request.Form["firstname"];
        lastName = Request.Form["lastname"];        
        message = FetchCalculation();  
    }
    
    private static string? FetchCalculation()
    {
        var task = RestClient.GetAsync<string>(new RestRequest("Calculation"));
        return task.Result;
    }
}