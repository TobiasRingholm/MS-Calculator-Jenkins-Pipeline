using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;
using MySqlConnector;
using System.Diagnostics;
using SharedModel;
using Monitoring;


namespace CalculationHistoryService.Controllers;

[ApiController]
[Route("[controller]")]
public class CalculationHistoryController : ControllerBase
{
    private IDbConnection calculationCache =
        new MySqlConnection("Server=cache-db;Database=cache-database;Uid=div-cache;Pwd=C@ch3d1v;");

    public CalculationHistoryController()
    {
        calculationCache.Open();
        var tables = calculationCache.Query<string>("SHOW TABLES LIKE 'calculations'");
        if (!tables.Any())
        {
            calculationCache.Execute(
                "CREATE TABLE calculations (ID INT NOT NULL PRIMARY KEY AUTO_INCREMENT, value1 DOUBLE NOT NULL,value2 DOUBLE NOT NULL,result DOUBLE NOT NULL,mathOperator VARCHAR(255) NOT NULL)");
        }
    }

    [HttpGet]
    public ActionResult<List<Calculation>> Get()
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered Get All Calculations From DB method");

        var allCalculations = calculationCache.Query<Calculation>("SELECT * FROM calculations ORDER BY ID DESC").ToList();

        return allCalculations;
    }


    [HttpPost]
    public void Post([FromQuery] double numberA, [FromQuery] double numberB, [FromQuery] double result,
        [FromQuery] string mathOperator)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity();
        MonitoringService.Log.Here().Debug("Entered PostCalculation to DB method");
        calculationCache.Execute(
            "INSERT INTO calculations (value1, value2, result, mathOperator) VALUES (@numberA, @numberB, @result, @mathOperator)",
            new { numberA = numberA, numberB = numberB, result = result, mathOperator = mathOperator });
    }
}