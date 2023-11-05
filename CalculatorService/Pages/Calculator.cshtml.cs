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
        Regex validExpression = new Regex(@"^[\d\+\-\*\/]*$");
        return validExpression.IsMatch(input);
    }

private static double CalculateExpression(string input)
{
    double result = 0;
    double currentNumber = 0;
    char operation = '+';

    // Fjern alle mellemrum fra input for at undgå fejl ved parsing
    input = input.Replace(" ", string.Empty);

    for (int i = 0; i < input.Length; i++)
    {
        if (char.IsDigit(input[i]) || input[i] == '.')
        {
            // Byg det nuværende tal karakter for karakter
            string numberStr = input[i].ToString();
            while (i + 1 < input.Length && (char.IsDigit(input[i + 1]) || input[i + 1] == '.'))
            {
                numberStr += input[++i];
            }

            // Konverter strengen til et tal
            currentNumber = double.Parse(numberStr);
            
            // Udfør den forrige operation
            if (operation == '+')
            {
                result += currentNumber;
            }
            else if (operation == '-')
            {
                result -= currentNumber;
            }

            // Nulstil det nuværende tal
            currentNumber = 0;
        }
        else if (input[i] == '+' || input[i] == '-')
        {
            // Gem den nuværende operation til næste gang
            operation = input[i];
        }
        else
        {
            // Håndter ugyldige karakterer
            throw new ArgumentException("Invalid character encountered in expression.");
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
