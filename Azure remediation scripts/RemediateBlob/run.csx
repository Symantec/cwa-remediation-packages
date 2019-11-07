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



public static void PublicAccess(string resource_id, ILogger log,string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "?api-version=2019-04-01";

    log.LogInformation("URL for API: "+endpoint);
    var httpCl = new HttpClient();
    httpCl.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);

    var httpResponse = new HttpResponseMessage();
    string httpResponseBody = "";
    httpResponse = httpCl.GetAsync(endpoint).Result;
    httpResponseBody = httpResponse.Content.ReadAsStringAsync().Result.ToString();
    log.LogInformation("Get response for Blob Service is: ");
    log.LogInformation(httpResponseBody);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.publicAccess = "None";


    var httpClient = new HttpClient();
   httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);
   string p = JsonConvert.SerializeObject(c);
   log.LogInformation(p.ToString());
   var content = new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json");
   var response = httpClient.PutAsync(endpoint, content).Result;
   string result = response.Content.ReadAsStringAsync().Result.ToString();
    log.LogInformation(content.ToString());
   log.LogInformation("Blob setting:" + result);
   var statuscode = response.StatusCode.ToString();

   log.LogInformation("Status code for API:" + statuscode);

}


public static void Run(Tuple<string, string, string, string, string> tuple1, ILogger log)
{
 
   log.LogInformation("Activity function started...");
   string module_id = tuple1.Item1;
   string resource_id = tuple1.Item2;
   string stoken = tuple1.Item3;
   string sub = tuple1.Item4;
   string res_g = tuple1.Item5;
   log.LogInformation("Module ID: " + module_id);

   log.LogInformation("Token: " + stoken);

 
   string URL= resource_id;
    string[] s_URL = URL.Split("//");
    string s1 = s_URL[1].ToString();
    string[] s2 = s1.Split(".");
    log.LogInformation(s2[0]);
    int index=URL.LastIndexOf("/");
    int len = URL.Length;
    string a=URL.Substring(index+1);
     log.LogInformation(URL);
    log.LogInformation(a);

   log.LogInformation("Remediation started for resource with ID: ");
   
    resource_id = "/subscriptions/"+ sub +"/resourceGroups/" + res_g + "/providers/Microsoft.Storage/storageAccounts/" + s2[0] + "/blobServices/default/containers/" + a;
    log.LogInformation(resource_id);
    if(module_id == "PublicAccess")
    {
    PublicAccess(resource_id,log,stoken);
    }

    
}