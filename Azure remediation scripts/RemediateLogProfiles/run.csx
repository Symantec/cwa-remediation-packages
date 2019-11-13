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

using System;
using System.Net;
using System.Net.Http;
using System.Configuration;
using System.Security.Claims; 
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static string getLogProfile(string endpoint,ILogger log,string stoken)
{
   var httpCl = new HttpClient();
   httpCl.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);
   var httpResponse = new HttpResponseMessage();
   string httpResponseBody = "";
   httpResponse = httpCl.GetAsync(endpoint).Result;
   httpResponse.EnsureSuccessStatusCode();
   httpResponseBody = httpResponse.Content.ReadAsStringAsync().Result.ToString();
   return httpResponseBody;
} 

public static void setLogProfile(string endpoint,ILogger log, string stoken, dynamic c)
{
   var httpClient = new HttpClient();
   httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);
   var content = new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json");
   var response = httpClient.PutAsync(endpoint, content).Result;
   string result = response.Content.ReadAsStringAsync().Result.ToString();
   log.LogInformation("Log Profile setting:" + result);
   var statuscode = response.StatusCode.ToString();

   log.LogInformation("Status code for API:" + statuscode);
}
public static void Log_Retention(string resource_id, ILogger log,string stoken){

   
   string endpoint = "https://management.azure.com";
   endpoint = endpoint + resource_id + "?api-version=2016-03-01";
   log.LogInformation("URL for API: "+endpoint);

   string httpResponseBody = getLogProfile(endpoint,log,stoken);

   dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
   c.properties.retentionPolicy.enabled = true;
   c.properties.retentionPolicy.days = 365;
  
   setLogProfile(endpoint,log,stoken,c);

}

public static void Run(Tuple<string, string, string> tuple1, ILogger log)
{
   log.LogInformation("Activity function started...");
   string module_id = tuple1.Item1;
   string resource_id = tuple1.Item2;
   string stoken = tuple1.Item3;
   
   log.LogInformation("Remediation started for resource with ID: ");
   log.LogInformation(resource_id);

  log.LogInformation("Token: " + stoken);
  Log_Retention(resource_id,log,stoken);


}