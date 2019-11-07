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


public static string GetVirtualNetwork(string resource_id, ILogger log, string stoken, string endpoint)
{
   log.LogInformation("URL for API: "+endpoint);
    var httpCl = new HttpClient();
    httpCl.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);

    var httpResponse = new HttpResponseMessage();
    string httpResponseBody = "";
    httpResponse = httpCl.GetAsync(endpoint).Result;
    httpResponseBody = httpResponse.Content.ReadAsStringAsync().Result.ToString();
    log.LogInformation("Get response for Virtual Networks is: ");
    log.LogInformation(httpResponseBody);
    return httpResponseBody;
}
      
public static void setVirtualNetwork(dynamic c,string stoken,ILogger log,string endpoint){

   var httpClient = new HttpClient();
   httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);
   string p = JsonConvert.SerializeObject(c);
   log.LogInformation(p.ToString());
   var content = new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json");
   var response = httpClient.PutAsync(endpoint, content).Result;
   string result = response.Content.ReadAsStringAsync().Result.ToString();
    log.LogInformation(content.ToString());
   log.LogInformation("Virtual Network setting:" + result);
   var statuscode = response.StatusCode.ToString();

   log.LogInformation("Status code for API:" + statuscode);

}

public static void EnableDdos(string resource_id, ILogger log,string stoken)
{
   string endpoint = "https://management.azure.com/";
   endpoint = endpoint + resource_id + "?api-version=2019-06-01";

   string httpResponseBody = GetVirtualNetwork(resource_id,log,stoken,endpoint);

   dynamic c = JsonConvert.DeserializeObject(httpResponseBody);

    setVirtualNetwork(c,stoken,log,endpoint);

}
public static void deletePeering(string resource_id,string name, string stoken,ILogger log)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "/virtualNetworkPeerings/" + name + "?api-version=2019-06-01";
    log.LogInformation("URL for API: "+endpoint);
    var httpCl = new HttpClient();
    httpCl.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);

    var httpResponse = new HttpResponseMessage();
    string httpResponseBody = "";
    httpResponse = httpCl.DeleteAsync(endpoint).Result;
    httpResponseBody = httpResponse.Content.ReadAsStringAsync().Result.ToString();

}
public static void NoPeerings(string resource_id, ILogger log,string stoken)
{
   string endpoint = "https://management.azure.com/";
   endpoint = endpoint + resource_id + "?api-version=2019-06-01";

   string httpResponseBody = GetVirtualNetwork(resource_id,log,stoken,endpoint);

   dynamic c = JsonConvert.DeserializeObject(httpResponseBody);

   int len = c.properties.virtualNetworkPeerings.Count;
    for (int i=0;i<len;i++)
    {
            string name = c.properties.virtualNetworkPeerings[i].name; 
            log.LogInformation(name);
            deletePeering(resource_id,name,stoken,log);
    }

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

    if(module_id == "NoPeerings")
        NoPeerings(resource_id,log,stoken);

}