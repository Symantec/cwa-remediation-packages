#r "../bin/Microsoft.Azure.EventGrid.dll"
#r "Microsoft.Azure.WebJobs.Extensions.DurableTask"

using Microsoft.Azure.EventGrid.Models;


public static async Task Run(
    EventGridEvent gridevent,
    DurableOrchestrationClient starter,
    ILogger log)
{
    log.LogInformation("Function was triggered by event ");
    var event_data = gridevent.Data.ToString();
    log.LogInformation(event_data);
    
    string instanceId = await starter.StartNewAsync("OrchestratorReme", event_data);
    log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
    
}
