using PatientTriagePortal.Services;
using Xunit;

namespace PatientTriagePortal.Tests;

public class AITriageServiceTests
{
    [Fact]
    public void EvaluateSymptoms_GivenChestPain_ReturnsEmergency()
    {
        // Arrange
        var service = new AITriageService();
        var symptom = "I have severe chest pain and shortness of breath.";

        // Act
        var result = service.EvaluateSymptoms(symptom);

        // Assert
        Assert.Equal("Emergency", result.TriageLevel);
        Assert.Equal("Cardiology / ER", result.Department);
    }

    [Fact]
    public void EvaluateSymptoms_GivenFever_ReturnsRoutine()
    {
        // Arrange
        var service = new AITriageService();
        var symptom = "I have a mild fever and a cold.";

        // Act
        var result = service.EvaluateSymptoms(symptom);

        // Assert
        Assert.Equal("Routine", result.TriageLevel);
        Assert.Equal("General Practice", result.Department);
    }
}
