using Microsoft.AspNetCore.SignalR;
using ClinicalTelemetryAPI.Models;

namespace ClinicalTelemetryAPI.Hubs;

public class TelemetryHub : Hub
{
    // Clients will connect to this hub and listen for 'ReceiveTelemetryUpdate'
}
