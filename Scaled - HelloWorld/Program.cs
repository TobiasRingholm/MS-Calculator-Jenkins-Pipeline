// See https://aka.ms/new-console-template for more information
using RestSharp;

public class Program
{
    private static readonly RestClient RestClient = new RestClient("http://calculation-service/");
    
    public static void Main(string[] args)
    {
        while (true)
        {
            Thread.Sleep(3000);
            
            var calculation = FetchCalculation(); 
            
            Console.WriteLine($"{calculation}");
        }
        // ReSharper disable once FunctionNeverReturns
    }
    
    private static string? FetchCalculation()
    {
        var task = RestClient.GetAsync<string>(new RestRequest("Calculation"));
        return task.Result;
    }
    
}


