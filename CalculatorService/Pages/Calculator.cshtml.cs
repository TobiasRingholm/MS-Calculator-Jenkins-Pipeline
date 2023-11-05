using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestSharp;
using System.Text.RegularExpressions;

namespace CalculatorService.Pages;

public class Calculator : PageModel
{
    private static readonly RestClient RestClient = new RestClient("http://calculation-service/");
    
    [BindProperty]
    public string Input { get; set; }
    
    public void OnGet()
    {
    }

    public IActionResult OnPostCalculate()
    {
        if (string.IsNullOrWhiteSpace(Input))
        {
            return Content("Input cannot be empty");
        }

        if (!IsValidExpression(Input))
        {
            return Content("Invalid input");
        }

        try
        {
            var result = CalculateExpression(Input);
            return Content(result.ToString());
        }
        catch (Exception ex)
        {
            // Log exception
            return Content("Error in calculation: " + ex.Message);
        }
    }
    
    private static bool IsValidExpression(string input)
    {
        // En simpel regex for at checke for kun tal og tilladte operatorer (+ og -)
        Regex validExpression = new Regex(@"^[\d\+\-]*$");
        return validExpression.IsMatch(input);
    }
    
    private static double CalculateExpression(string input)
    {
        // Du kan udvide denne funktion til at håndtere mere komplekse udtryk og flere operationer
        // For nu kan den kun håndtere addition og subtraktion med hele tal
        double result = 0;
        double currentNumber = 0;
        char currentOp = '+';

        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsDigit(input[i]))
            {
                currentNumber = (currentNumber * 10) + (input[i] - '0');
            }

            if (!char.IsDigit(input[i]) || i == input.Length - 1)
            {
                if (currentOp == '+')
                    result += currentNumber;
                else if (currentOp == '-')
                    result -= currentNumber;

                currentNumber = 0;
                if (i != input.Length - 1) 
                    currentOp = input[i];
            }
        }
        
        return result;
    }
    
    private static string? FetchCalculation()
    {
        var task = RestClient.GetAsync<string>(new RestRequest("Calculation"));
        return task.Result;
    }
}
