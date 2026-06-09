using System.Diagnostics;
using System.Text.Json;
using Polly;
using PharmacyBenefitService.Data;
using PharmacyBenefitService.Models;

namespace PharmacyBenefitService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var apiKey = _configuration["InsuranceAPIKey"];
        _logger.LogInformation("Pharmacy Benefit Service started. Using API Key: {KeyMask}***.", apiKey?.Substring(0, 4));

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => 
            {
                _logger.LogWarning("Python execution failed. Retrying... Attempt {Attempt}", retryAttempt);
                return TimeSpan.FromSeconds(2);
            });

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PharmacyDbContext>();

            var pendingPrescription = dbContext.PrescriptionQueue.FirstOrDefault();

            if (pendingPrescription == null)
            {
                // No items in queue, wait and poll again
                await Task.Delay(5000, stoppingToken);
                continue;
            }

            var jsonInput = JsonSerializer.Serialize(pendingPrescription);
            _logger.LogInformation("Evaluating prescription for Patient: {PatientId}, Medication: {Medication}", pendingPrescription.PatientId, pendingPrescription.MedicationName);

            try
            {
                var pythonResult = await retryPolicy.ExecuteAsync(() => RunPythonEngineAsync(jsonInput, stoppingToken));
                
                if (!string.IsNullOrWhiteSpace(pythonResult))
                {
                    var result = JsonSerializer.Deserialize<ApprovalResult>(pythonResult);
                    _logger.LogInformation("Result: {Status} | Reason: {Reason}", result?.Status, result?.Reason);

                    // Audit Logging
                    dbContext.ClaimsAudits.Add(new ClaimsAudit
                    {
                        PatientId = pendingPrescription.PatientId,
                        MedicationName = pendingPrescription.MedicationName,
                        Cost = pendingPrescription.Cost,
                        Status = result?.Status ?? "Unknown",
                        Reason = result?.Reason ?? "No reason provided",
                        ProcessedAt = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing prescription after retries. Moving to DLQ.");
                
                // Dead Letter Queue
                dbContext.ManualReviews.Add(new ManualReview
                {
                    PatientId = pendingPrescription.PatientId,
                    MedicationName = pendingPrescription.MedicationName,
                    Cost = pendingPrescription.Cost,
                    ErrorReason = ex.Message,
                    FailedAt = DateTime.UtcNow
                });
            }

            // Remove from queue in both success and fail scenarios
            dbContext.PrescriptionQueue.Remove(pendingPrescription);
            await dbContext.SaveChangesAsync(stoppingToken);
            
            await Task.Delay(2000, stoppingToken); // Small delay for readability
        }
        
        _logger.LogInformation("Pharmacy Benefit Service shutting down.");
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

public record ApprovalResult(string Status, string Reason);
