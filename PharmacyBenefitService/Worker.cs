using System.Diagnostics;
using System.Text.Json;
using Polly;

namespace PharmacyBenefitService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var pendingPrescriptions = new List<Prescription>
        {
            new("P101", "Amoxicillin", 25.50m),
            new("P102", "Humira", 6000.00m),
            new("P103", "Lisinopril", 15.00m),
            new("P104", "Oxycodone", 80.00m),
            new("P105", "Insulin Glargine", 350.00m)
        };

        var apiKey = _configuration["InsuranceAPIKey"];
        _logger.LogInformation("Pharmacy Benefit Service started. Using API Key: {KeyMask}***. Processing {Count} prescriptions...", 
            apiKey?.Substring(0, 4), pendingPrescriptions.Count);

        // Define Polly Retry Policy
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => 
            {
                _logger.LogWarning("Python execution failed. Retrying... Attempt {Attempt}", retryAttempt);
                return TimeSpan.FromSeconds(2);
            });

        foreach (var prescription in pendingPrescriptions)
        {
            if (stoppingToken.IsCancellationRequested) break;

            var jsonInput = JsonSerializer.Serialize(prescription);
            _logger.LogInformation("Evaluating prescription for Patient: {PatientId}, Medication: {Medication}", prescription.PatientId, prescription.MedicationName);

            try
            {
                var pythonResult = await retryPolicy.ExecuteAsync(() => RunPythonEngineAsync(jsonInput, stoppingToken));
                
                if (!string.IsNullOrWhiteSpace(pythonResult))
                {
                    var result = JsonSerializer.Deserialize<ApprovalResult>(pythonResult);
                    _logger.LogInformation("Result: {Status} | Reason: {Reason}", result?.Status, result?.Reason);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing prescription after retries.");
            }
            
            await Task.Delay(2000, stoppingToken); // Small delay for readability in logs
        }
        
        _logger.LogInformation("Finished processing queue. Shutting down.");
        Environment.Exit(0);
    }

    private async Task<string> RunPythonEngineAsync(string jsonArgs, CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "python",
            Arguments = $"insurance_rules.py \"{jsonArgs.Replace("\"", "\\\"")}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
        };

        using var process = Process.Start(startInfo);
        if (process == null) throw new InvalidOperationException("Failed to start python process.");

        string output = await process.StandardOutput.ReadToEndAsync(cancellationToken);
        string error = await process.StandardError.ReadToEndAsync(cancellationToken);
        
        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            var errorMsg = $"Python engine exited with code {process.ExitCode}. Error: {error}";
            _logger.LogError(errorMsg);
            throw new Exception(errorMsg);
        }

        return output;
    }
}

public record Prescription(string PatientId, string MedicationName, decimal Cost);
public record ApprovalResult(string Status, string Reason);
