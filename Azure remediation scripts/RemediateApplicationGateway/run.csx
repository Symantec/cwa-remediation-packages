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


public static string GetApplicationGateway(string resource_id, ILogger log, string stoken, string endpoint)
{
   log.LogInformation("URL for API: "+endpoint);
    var httpCl = new HttpClient();
    httpCl.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);

    var httpResponse = new HttpResponseMessage();
    string httpResponseBody = "";
    httpResponse = httpCl.GetAsync(endpoint).Result;
    httpResponseBody = httpResponse.Content.ReadAsStringAsync().Result.ToString();
    log.LogInformation("Get response for Application Gateway is: ");
    log.LogInformation(httpResponseBody);
    return httpResponseBody;
}
      
public static void setApplicationGateway(dynamic c,string stoken,ILogger log,string endpoint){

   var httpClient = new HttpClient();
   httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);
   string p = JsonConvert.SerializeObject(c);
   log.LogInformation(p.ToString());
   var content = new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json");
   var response = httpClient.PutAsync(endpoint, content).Result;
   string result = response.Content.ReadAsStringAsync().Result.ToString();
    log.LogInformation(content.ToString());
   log.LogInformation("SQL setting:" + result);
   var statuscode = response.StatusCode.ToString();

   log.LogInformation("Status code for API:" + statuscode);

}

public static void EnableWAF(string resource_id, ILogger log,string stoken)
{
   string endpoint = "https://management.azure.com/";
   endpoint = endpoint + resource_id + "?api-version=2019-06-01";
   string httpResponseBody = GetApplicationGateway(resource_id,log,stoken,endpoint);

   dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    string s = c.name.ToString();
    if(c.properties.sku.tier!= "WAF_v2")
    {
        log.LogInformation("Tier not set to WAF");
        return;
    }
    if(c.properties.webApplicationFirewallConfiguration == null)
    {
        waf w = new waf();
        
        var json = JsonConvert.SerializeObject(w);
        string ss = json.ToString();
        log.LogInformation("DoesNotExist");
        string str = c.ToString();
        string name = "\"webApplicationFirewallConfiguration\": ";
        ss = ss.Insert(0,name);
        ss = String.Concat(ss,',');
        int index = str.IndexOf("properties");
        index +=15;
        str = str.Insert(index,ss);
      log.LogInformation(str);
        c = JObject.Parse(str);
      
    
    } 
    else
    {
      c.properties.webApplicationFirewallConfiguration.enabled = true;
    }
    setApplicationGateway(c,stoken,log,endpoint);

}

public static void EnablePrevention(string resource_id, ILogger log,string stoken)
{
   string endpoint = "https://management.azure.com/";
   endpoint = endpoint + resource_id + "?api-version=2019-06-01";
   string httpResponseBody = GetApplicationGateway(resource_id,log,stoken,endpoint);

   dynamic c = JsonConvert.DeserializeObject(httpResponseBody);

    string s = c.name.ToString();
    if(c.properties.sku.tier!= "WAF_v2")
    {
        log.LogInformation("resource: " + s +" tier is not set to WAF_V2");
        log.LogInformation("Could not proceed for remediation!!");
        return;
    }

    if(c.properties.webApplicationFirewallConfiguration == null)
    {
        waf w = new waf();
        
        var json = JsonConvert.SerializeObject(w);
        string ss = json.ToString();
        log.LogInformation("DoesNotExist");
        string str = c.ToString();
        string name = "\"webApplicationFirewallConfiguration\": ";
        ss = ss.Insert(0,name);
        ss = String.Concat(ss,',');
        int index = str.IndexOf("properties");
        index +=15;
        str = str.Insert(index,ss);
      log.LogInformation(str);
        c = JObject.Parse(str);
      
    
    } 
    else
    {
      c.properties.webApplicationFirewallConfiguration.enabled = true;
    }
    c.properties.webApplicationFirewallConfiguration.firewallMode = "Prevention";
    setApplicationGateway(c,stoken,log,endpoint);

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

   if(module_id=="EnableWAF")
      EnableWAF(resource_id,log,stoken);
      
    else
        EnablePrevention(resource_id,log,stoken);
    /*test*/
}
