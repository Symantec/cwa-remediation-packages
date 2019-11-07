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
//#load "properties.csx"

using System;
using System.Net;
using System.Net.Http;
using System.Configuration;
using System.Security.Claims; 
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public static string GetPolicy(string resource_id, ILogger log, string stoken, string endpoint)
{
   log.LogInformation("URL for API: "+endpoint);
    var httpCl = new HttpClient();
    httpCl.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);

    var httpResponse = new HttpResponseMessage();
    string httpResponseBody = "";
    httpResponse = httpCl.GetAsync(endpoint).Result;
    httpResponseBody = httpResponse.Content.ReadAsStringAsync().Result.ToString();
    log.LogInformation("Get response: ");
    log.LogInformation(httpResponseBody);
    return httpResponseBody;
}
      
public static void SetPolicy(dynamic c,string stoken,ILogger log,string endpoint){

   var httpClient = new HttpClient();
   httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);
   string p = JsonConvert.SerializeObject(c);
   log.LogInformation(p.ToString());
   var content = new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json");
   var response = httpClient.PutAsync(endpoint, content).Result;
   string result = response.Content.ReadAsStringAsync().Result.ToString();
    log.LogInformation(content.ToString());
   log.LogInformation("Security Policy setting:" + result);
   var statuscode = response.StatusCode.ToString();

   log.LogInformation("Status code for API:" + statuscode);

}



public static void Email_Owner(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "?api-version=2015-06-01-preview";
    string httpResponseBody = GetPolicy(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.securityContactConfiguration.securityContactPhone = "9898989898";
    c.properties.securityContactConfiguration.securityContactEmails = "a@b.com";
    c.properties.securityContactConfiguration.sendToAdminOn = true;   
    SetPolicy(c,stoken,log,endpoint);
}

public static void Phone_Set(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "?api-version=2015-06-01-preview";
    string httpResponseBody = GetPolicy(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.securityContactConfiguration.securityContactPhone = "9898989898";
    SetPolicy(c,stoken,log,endpoint);
}
public static void Email_Set(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "?api-version=2015-06-01-preview";
    string httpResponseBody = GetPolicy(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.securityContactConfiguration.securityContactEmails = "a@b.com";
    SetPolicy(c,stoken,log,endpoint);
}
public static void Vulnerability_Assessment(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "?api-version=2015-06-01-preview";
    string httpResponseBody = GetPolicy(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.recommendations.vulnerabilityAssessment = "On";
    SetPolicy(c,stoken,log,endpoint);
}
public static void AAC_Enable(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "?api-version=2015-06-01-preview";
    string httpResponseBody = GetPolicy(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.recommendations.appWhitelisting = "On";
    SetPolicy(c,stoken,log,endpoint);
}
public static void NGF_Enable(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "?api-version=2015-06-01-preview";
    string httpResponseBody = GetPolicy(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.recommendations.ngfw = "On";
    SetPolicy(c,stoken,log,endpoint);
}

public static void JIT(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "?api-version=2015-06-01-preview";
    string httpResponseBody = GetPolicy(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.recommendations.jitNetworkAccess = "On";
    SetPolicy(c,stoken,log,endpoint);
}
public static void Disk_Encryption(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "?api-version=2015-06-01-preview";
    string httpResponseBody = GetPolicy(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.recommendations.diskEncryption = "On";
    SetPolicy(c,stoken,log,endpoint);
}
public static void WAF_Enable(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "?api-version=2015-06-01-preview";
    string httpResponseBody = GetPolicy(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.recommendations.waf = "On";
    SetPolicy(c,stoken,log,endpoint);
}

public static void System_Update(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "?api-version=2015-06-01-preview";
    string httpResponseBody = GetPolicy(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.recommendations.patch = "On";
    SetPolicy(c,stoken,log,endpoint);
}

public static void Security_Config(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "?api-version=2015-06-01-preview";
    string httpResponseBody = GetPolicy(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.recommendations.baseline = "On";
    SetPolicy(c,stoken,log,endpoint);
}

public static void Endpoint_Protection(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "?api-version=2015-06-01-preview";
    string httpResponseBody = GetPolicy(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.recommendations.antimalware = "On";
    SetPolicy(c,stoken,log,endpoint);
}

public static void Monitoring_Agent(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "?api-version=2015-06-01-preview";
    string httpResponseBody = GetPolicy(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
    c.properties.logCollection = "On";
    SetPolicy(c,stoken,log,endpoint);
}

public static void Email_User(string resource_id, ILogger log, string stoken)
{
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "?api-version=2015-06-01-preview";
    string httpResponseBody = GetPolicy(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
     c.properties.securityContactConfiguration.securityContactPhone = "9898989898";
    c.properties.securityContactConfiguration.securityContactEmails = "a@b.com";
    c.properties.securityContactConfiguration.areNotificationsOn = true;
    SetPolicy(c,stoken,log,endpoint);
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

   if(module_id=="Email_Owner")
      Email_Owner(resource_id,log,stoken);
      
   else if(module_id=="Phone_Set")
        Phone_Set(resource_id,log,stoken);

   else if(module_id=="Email_Set")
        Email_Set(resource_id,log,stoken);

   else if(module_id=="Vulnerability_Assessment")
        Vulnerability_Assessment(resource_id,log,stoken);

   else if(module_id=="AAC_Enable")
        AAC_Enable(resource_id,log,stoken);   

   else if(module_id=="NGF_Enable")
        NGF_Enable(resource_id,log,stoken);  
 
   else if(module_id=="JIT")
        JIT(resource_id,log,stoken);   

   else if(module_id=="Disk_Encryption")
        Disk_Encryption(resource_id,log,stoken);  

   else if(module_id=="WAF_Enable")
        WAF_Enable(resource_id,log,stoken);     

   else if(module_id=="Email_User")
        Email_User(resource_id,log,stoken);   
           
   else if(module_id=="Security_Config")
        Security_Config(resource_id,log,stoken); 
   
   else if(module_id=="System_Update")
        System_Update(resource_id,log,stoken); 

    else if(module_id=="Endpoint_Protection")
        Endpoint_Protection(resource_id,log,stoken); 

    else if(module_id=="Monitoring_Agent")
        Monitoring_Agent(resource_id,log,stoken); 

}