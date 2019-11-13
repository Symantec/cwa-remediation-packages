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

public static string GetAuditing(string resource_id, ILogger log, string stoken, string endpoint)
{
   log.LogInformation("URL for API: "+endpoint);
    var httpCl = new HttpClient();
    httpCl.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);

    var httpResponse = new HttpResponseMessage();
    string httpResponseBody = "";
    httpResponse = httpCl.GetAsync(endpoint).Result;
    httpResponseBody = httpResponse.Content.ReadAsStringAsync().Result.ToString();
    log.LogInformation("Get response for SQL Server is: ");
    log.LogInformation(httpResponseBody);
    return httpResponseBody;
}
      
public static void SetAuditing(dynamic c,string stoken,ILogger log,string endpoint){

   var httpClient = new HttpClient();
   httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);
   string p = JsonConvert.SerializeObject(c);
   log.LogInformation(p.ToString());
   var content = new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json");
   var response = httpClient.PutAsync(endpoint, content).Result;
   string result = response.Content.ReadAsStringAsync().Result.ToString();
    log.LogInformation(content.ToString());
   log.LogInformation("SQL Server setting:" + result);
   var statuscode = response.StatusCode.ToString();

   log.LogInformation("Status code for API:" + statuscode);

}



public static void EnableAuditing(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "/auditingSettings/default?api-version=2017-03-01-preview";
    string httpResponseBody = GetAuditing(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.state = "Enabled";
    SetAuditing(c,stoken,log,endpoint);
}

public static void AuditingDays(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "/auditingSettings/default?api-version=2017-03-01-preview";
    string httpResponseBody = GetAuditing(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.retentionDays = 95;
    SetAuditing(c,stoken,log,endpoint);
}

public static string GetAlerts(string resource_id, ILogger log, string stoken, string endpoint)
{
   log.LogInformation("URL for API: "+endpoint);
    var httpCl = new HttpClient();
    httpCl.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);

    var httpResponse = new HttpResponseMessage();
    string httpResponseBody = "";
    httpResponse = httpCl.GetAsync(endpoint).Result;
    httpResponseBody = httpResponse.Content.ReadAsStringAsync().Result.ToString();
    log.LogInformation("Get response for SQL Server is: ");
    log.LogInformation(httpResponseBody);
    return httpResponseBody;
}
      
public static void SetAlerts(dynamic c,string stoken,ILogger log,string endpoint){

   var httpClient = new HttpClient();
   httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);
   string p = JsonConvert.SerializeObject(c);
   log.LogInformation(p.ToString());
   var content = new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json");
   var response = httpClient.PutAsync(endpoint, content).Result;
   string result = response.Content.ReadAsStringAsync().Result.ToString();
    log.LogInformation(content.ToString());
   log.LogInformation("SQL Server setting:" + result);
   var statuscode = response.StatusCode.ToString();

   log.LogInformation("Status code for API:" + statuscode);

}



public static void EnableEmail(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "/securityAlertPolicies/Default?api-version=2017-03-01-preview";
    string httpResponseBody = GetAlerts(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.emailAccountAdmins = true;
    if(c.properties.storageEndpoint == "")
    c.properties.Property("storageEndpoint").Remove();
    if(c.properties.storageAccountAccessKey == "")
    c.properties.Property("storageAccountAccessKey").Remove();
    SetAlerts(c,stoken,log,endpoint);
}

public static void EnableAlerts(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "/securityAlertPolicies/Default?api-version=2017-03-01-preview";
    string httpResponseBody = GetAlerts(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.emailAddresses[0] = "a@b.com";
    SetAlerts(c,stoken,log,endpoint);
}
public static void EnableThreatD(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "/securityAlertPolicies/Default?api-version=2017-03-01-preview";
    string httpResponseBody = GetAlerts(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.state = "Enabled";
    if(c.properties.storageEndpoint == "")
    c.properties.Property("storageEndpoint").Remove();
    if(c.properties.storageAccountAccessKey == "")
    c.properties.Property("storageAccountAccessKey").Remove();
    SetAlerts(c,stoken,log,endpoint);
}

public static void ThreatD_Types(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "/securityAlertPolicies/Default?api-version=2017-03-01-preview";
    string httpResponseBody = GetAlerts(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    
    c.properties.disabledAlerts = null;
    if(c.properties.storageEndpoint == "")
    c.properties.Property("storageEndpoint").Remove();
    if(c.properties.storageAccountAccessKey == "")
    c.properties.Property("storageAccountAccessKey").Remove();
    SetAlerts(c,stoken,log,endpoint);
}

public static void ThreatD_Days(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "/securityAlertPolicies/Default?api-version=2017-03-01-preview";
    string httpResponseBody = GetAlerts(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.retentionDays = 95;
    if(c.properties.storageEndpoint == "")
    c.properties.Property("storageEndpoint").Remove();
    if(c.properties.storageAccountAccessKey == "")
    c.properties.Property("storageAccountAccessKey").Remove();
    SetAlerts(c,stoken,log,endpoint);
}
public static void Run(Tuple<string, string, string> tuple1, ILogger log)
{
   //test1
   log.LogInformation("Activity function RemediateSQLDB started...");
   string module_id = tuple1.Item1;
   log.LogInformation("Calling module: "+ module_id);
   string resource_id = tuple1.Item2;
   string stoken = tuple1.Item3;
   log.LogInformation("Remediation started for resource with ID: ");
   log.LogInformation(resource_id);

   if (module_id == "EnableAuditing"){
       EnableAuditing(resource_id, log, stoken);
   }
    
   if (module_id == "AuditingDays"){
       AuditingDays(resource_id, log, stoken);
   }

   if (module_id == "EnableEmail"){
       EnableEmail(resource_id, log, stoken);
   }

   if (module_id == "EnableAlerts"){
       EnableAlerts(resource_id, log, stoken);
   }

   if (module_id == "EnableThreatD"){
       EnableThreatD(resource_id, log, stoken);
   }

   if (module_id == "ThreatD_Days"){
       ThreatD_Days(resource_id, log, stoken);
   }

   if (module_id == "ThreatD_Types"){
       ThreatD_Types(resource_id, log, stoken);
   }

}
