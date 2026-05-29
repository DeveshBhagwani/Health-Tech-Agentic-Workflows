using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientTriagePortal.Data;
using PatientTriagePortal.Models;
using PatientTriagePortal.Services;

namespace PatientTriagePortal.Controllers;

public class TriageController : Controller
{
    private readonly AppDbContext _context;
    private readonly AITriageService _aiService;
    private readonly ILogger<TriageController> _logger;

    public TriageController(AppDbContext context, AITriageService aiService, ILogger<TriageController> logger)
    {
        _context = context;
        _aiService = aiService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(PatientSymptom model)
    {
        if (ModelState.IsValid)
        {
            _logger.LogInformation("New symptom submission received for patient: {PatientName}", model.PatientName);

            // Simulate Agentic AI response
            var triageResult = _aiService.EvaluateSymptoms(model.SymptomDescription);
            
            model.TriageLevel = triageResult.TriageLevel;
            model.SuggestedDepartment = triageResult.Department;
            model.SubmittedAt = DateTime.UtcNow;

            _logger.LogInformation("AI Triage Result for patient {PatientName}: Level={Level}, Department={Department}", model.PatientName, model.TriageLevel, model.SuggestedDepartment);

            _context.Add(model);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Dashboard));
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var records = await _context.PatientSymptoms
            .OrderByDescending(p => p.SubmittedAt)
            .ToListAsync();
            
        return View(records);
    }
}
