/*
 * This function is not intended to be invoked directly. Instead it will be
 * triggered by an orchestrator function.
 * 
 * Before running this sample, please:
 * - create a Durable orchestration function
 * - create a Durable HTTP starter function
 */

#r "Microsoft.Azure.WebJobs.Extensions.DurableTask"
#r "Newtonsoft.Json"
#load "properties.csx"

using System;
using System.Net;
using System.Net.Http;
using System.Configuration;
using System.Security.Claims; 
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;




public static void SecureTransfer(string resource_id, ILogger log,string stoken)
{
   string endpoint = "https://management.azure.com/";
   storage_prop sp = new storage_prop();
   prop p = new prop();
   endpoint = endpoint + resource_id + "?api-version=2019-04-01";
   log.LogInformation("URL for API: "+endpoint);
   p.supportsHttpsTrafficOnly =  true;
    sp.properties = p; 
   var httpClient = new HttpClient();
   httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);
   log.LogInformation(JsonConvert.SerializeObject(sp));
   var content = new StringContent(JsonConvert.SerializeObject(sp), Encoding.UTF8, "application/json");
   var response = httpClient.PatchAsync(endpoint, content).Result;
  string result = response.Content.ReadAsStringAsync().Result.ToString();

  log.LogInformation("Content:" + content);
   log.LogInformation("SQL setting:" + result);

   var statuscode = response.StatusCode.ToString();
   log.LogInformation("Status code for API:" + statuscode);
}




public static void Run(Tuple<string, string, string> tuple1, ILogger log)
{
 
   log.LogInformation("Activity function started...");
   string module_id = tuple1.Item1;
   string resource_id = tuple1.Item2;
   string stoken = tuple1.Item3;
   log.LogInformation("Module ID: " + module_id);

   log.LogInformation("Token: " + stoken);
    
   log.LogInformation("Remediation started for resource with ID: ");
   log.LogInformation(resource_id);

    SecureTransfer(resource_id,log,stoken);
}
