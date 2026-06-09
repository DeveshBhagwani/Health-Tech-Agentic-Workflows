using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Asp.Versioning;
using ClinicalTelemetryAPI.Data;
using ClinicalTelemetryAPI.Models;
using ClinicalTelemetryAPI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ClinicalTelemetryAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class TelemetryController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly IHubContext<TelemetryHub> _hubContext;

    public TelemetryController(AppDbContext context, IMemoryCache cache, IHubContext<TelemetryHub> hubContext)
    {
        _context = context;
        _cache = cache;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> PostTelemetry([FromBody] TelemetryReading reading)
    {
        if (reading == null || string.IsNullOrWhiteSpace(reading.PatientId))
        {
            return BadRequest("Invalid telemetry data.");
        }

        if (reading.Timestamp == default)
        {
            reading.Timestamp = DateTime.UtcNow;
        }

        _context.TelemetryReadings.Add(reading);
        await _context.SaveChangesAsync();

        // Broadcast real-time update
        await _hubContext.Clients.All.SendAsync("ReceiveTelemetryUpdate", reading);

        return CreatedAtAction(nameof(GetSummary), new { patientId = reading.PatientId }, reading);
    }

    [HttpGet("{patientId}")]
    public async Task<IActionResult> GetReadings(string patientId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] bool? abnormalVitalsOnly = null)
    {
        var query = _context.TelemetryReadings.Where(t => t.PatientId == patientId);

        if (abnormalVitalsOnly == true)
        {
            query = query.Where(t => t.HeartRate < 60 || t.HeartRate > 100 || 
                                     t.BloodPressureSystolic > 130 || t.BloodPressureDiastolic > 80);
        }

        var totalRecords = await query.CountAsync();
        
        var readings = await query
            .OrderByDescending(t => t.Timestamp)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new 
        {
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Data = readings
        });
    }

    [HttpGet("{patientId}/summary")]
    public async Task<IActionResult> GetSummary(string patientId)
    {
        var cacheKey = $"TelemetrySummary_{patientId}";

        if (_cache.TryGetValue(cacheKey, out var cachedResult))
        {
            return Ok(cachedResult);
        }

        var cutoff = DateTime.UtcNow.AddHours(-24);

        var query = _context.TelemetryReadings
            .Where(t => t.PatientId == patientId && t.Timestamp >= cutoff);

        var count = await query.CountAsync();
        
        if (count == 0)
        {
            return NotFound($"No telemetry readings found for patient {patientId} in the last 24 hours.");
        }

        var averageHeartRate = await query.AverageAsync(t => t.HeartRate);
        var averageSystolic = await query.AverageAsync(t => t.BloodPressureSystolic);
        var averageDiastolic = await query.AverageAsync(t => t.BloodPressureDiastolic);

        var result = new
        {
            PatientId = patientId,
            Period = "Last 24 Hours",
            ReadingCount = count,
            AverageHeartRate = Math.Round(averageHeartRate, 1),
            AverageSystolic = Math.Round(averageSystolic, 1),
            AverageDiastolic = Math.Round(averageDiastolic, 1)
        };

        // Cache the result for 60 seconds
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
            
        _cache.Set(cacheKey, result, cacheOptions);

        return Ok(result);
    }
}
