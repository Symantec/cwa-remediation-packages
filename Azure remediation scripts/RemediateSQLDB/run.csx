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
//test compilation3

using System;
using System.Net;
using System.Net.Http;
using System.Configuration;
using System.Security.Claims; 
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


private static void Data_Encryption(string resource_id, ILogger log, string stoken){
   
   var httpClient = new HttpClient();
   httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);

   string endpoint = "https://management.azure.com";

   PropertiesEncrypt PropEnc = new PropertiesEncrypt();
   endpoint = endpoint + resource_id + "/transparentDataEncryption/current?api-version=2014-04-01";
   log.LogInformation("URL for API: "+endpoint);
   PropEnc.status =  "Enabled";
   EncryptionProp ep = new EncryptionProp();
   ep.properties = PropEnc;  
   var content = new StringContent(JsonConvert.SerializeObject(ep), Encoding.UTF8, "application/json");
   //log.LogInformation("content: ", content);
   var response = httpClient.PutAsync(endpoint,content).Result;
   string result = response.Content.ReadAsStringAsync().Result.ToString();

   log.LogInformation("SQL setting:" + result);
   string statuscode = response.StatusCode.ToString();

   log.LogInformation("Status code for API:" + statuscode);
}

private static void Enable_ThreatD(string resource_id, ILogger log, string stoken){

   var httpClient = new HttpClient();
   httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);
   
   string endpoint = "https://management.azure.com";

   Properties prop = new Properties();
   endpoint = endpoint + resource_id + "/securityAlertPolicies/default?api-version=2014-04-01";
   log.LogInformation("endpoint: " + endpoint);            
   prop.state = "Enabled";
   prop.emailAccountAdmins = "Enabled";
   prop.emailAddresses = "abc@symantec.com";
   propobj p = new propobj();
   p.properties = prop;
   var content = new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json");
   //log.LogInformation("content: ", content);
   var response = httpClient.PutAsync(endpoint,content).Result;
   string result = response.Content.ReadAsStringAsync().Result.ToString();

   log.LogInformation("SQL setting:" + result);
   string statuscode = response.StatusCode.ToString();

   log.LogInformation("Status code for API:" + statuscode);
}


private static void Threat_Retn90(string resource_id, ILogger log, string stoken){
   
   var httpClient = new HttpClient();
   httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);
   string endpoint = "https://management.azure.com";
   Properties prop = new Properties();
   endpoint = endpoint + resource_id + "/securityAlertPolicies/default?api-version=2014-04-01";
   log.LogInformation("endpoint: " + endpoint);            
   prop.state = "Enabled";
   prop.emailAccountAdmins = "Enabled";
   prop.emailAddresses = "abc@symantec.com";
   prop.retentionDays = 95;
   propobj p = new propobj();
   p.properties = prop;
   //string jsonContent1 = JsonConvert.SerializeObject(p);
   var content = new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json");
   //log.LogInformation("content: ", content);
   var response = httpClient.PutAsync(endpoint,content).Result;
   string result = response.Content.ReadAsStringAsync().Result.ToString();

   log.LogInformation("SQL setting:" + result);
   string statuscode = response.StatusCode.ToString();

   log.LogInformation("Status code for API:" + statuscode);
}

private static void Send_Alert(string resource_id, ILogger log, string stoken){

   var httpClient = new HttpClient();
   httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);
   string endpoint = "https://management.azure.com";
   Properties prop = new Properties();
   endpoint = endpoint + resource_id + "/securityAlertPolicies/default?api-version=2014-04-01";
   log.LogInformation("endpoint: " + endpoint);            
   prop.state = "Enabled";
   prop.emailAccountAdmins = "Enabled";
   prop.emailAddresses = "abc@symantec.com";
   propobj p = new propobj();
   p.properties = prop;
   //string jsonContent1 = JsonConvert.SerializeObject(p);test
   var content = new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json");
   //log.LogInformation("content: ", content);
   var response = httpClient.PutAsync(endpoint,content).Result;
   string result = response.Content.ReadAsStringAsync().Result.ToString();

   log.LogInformation("SQL setting:" + result);
   string statuscode = response.StatusCode.ToString();

   log.LogInformation("Status code for API:" + statuscode);
}

private static void Detection_Type_All(string resource_id, ILogger log, string stoken){

   var httpClient = new HttpClient();
   httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken); 
   string endpoint = "https://management.azure.com";
   Properties prop = new Properties();
   endpoint = endpoint + resource_id + "/securityAlertPolicies/default?api-version=2014-04-01";
   log.LogInformation("endpoint: " + endpoint);            
   prop.state = "Enabled";
   prop.emailAccountAdmins = "Enabled";
   prop.emailAddresses = "abc@symantec.com";
   prop.disabledAlerts = "";
   propobj p = new propobj();
   p.properties = prop;
   //string jsonContent1 = JsonConvert.SerializeObject(p);
   var content = new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json");
   //log.LogInformation("content: ", content);
   var response = httpClient.PutAsync(endpoint,content).Result;
   string result = response.Content.ReadAsStringAsync().Result.ToString();

   log.LogInformation("SQL setting:" + result);
   string statuscode = response.StatusCode.ToString();

   log.LogInformation("Status code for API:" + statuscode);
   
}

private static void Email_Service(string resource_id, ILogger log, string stoken){

   var httpClient = new HttpClient();
   httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken); 
   string endpoint = "https://management.azure.com";
   Properties prop = new Properties();
   endpoint = endpoint + resource_id + "/securityAlertPolicies/default?api-version=2014-04-01";
   log.LogInformation("endpoint: " + endpoint);            
   prop.state = "Enabled";
   prop.emailAccountAdmins = "Enabled";
   prop.emailAddresses = "abc@symantec.com";
   propobj p = new propobj();
   p.properties = prop;
   //string jsonContent1 = JsonConvert.SerializeObject(p);
   var content = new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json");
   //log.LogInformation("content: ", content);
   var response = httpClient.PutAsync(endpoint,content).Result;
   string result = response.Content.ReadAsStringAsync().Result.ToString();

   log.LogInformation("SQL setting:" + result);
   string statuscode = response.StatusCode.ToString();

   log.LogInformation("Status code for API:" + statuscode);
   
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

   if (module_id == "Data_Encryption"){
       Data_Encryption(resource_id, log, stoken);
   }
   
   if (module_id == "Enable_ThreatD"){
       Enable_ThreatD(resource_id, log, stoken);
   }

   if (module_id == "Threat_Retn90"){
       Threat_Retn90(resource_id, log, stoken);
   }

   if (module_id == "Send_Alert"){
       Send_Alert(resource_id, log, stoken);
   }

   if (module_id == "Detection_Type_All"){
       Detection_Type_All(resource_id, log, stoken);
   }

   if (module_id == "Email_Service"){
       Email_Service(resource_id, log, stoken);
   }
   
   
  /*test
   if (jsonContent != ""){
        log.LogInformation("jsonContent: ", jsonContent);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        
        log.LogInformation("content: ", content);
        var response = httpClient.PutAsync(endpoint, content).Result;
        string result = response.Content.ReadAsStringAsync().Result.ToString();

        log.LogInformation("SQL setting:" + result);
        string statuscode = response.StatusCode.ToString();

        log.LogInformation("Status code for API:" + statuscode);
   }
   else {
       log.LogInformation("Received blank jsonContent !!");
       log.LogInformation("Error while running module: "+ module_id);
   }
   */
}
