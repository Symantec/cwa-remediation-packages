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
/*test*/

using System;
using System.Net;
using System.Net.Http;
using System.Configuration;
using System.Security.Claims; 
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

private string Data_Encryption(string endpoint,string resource_id, ILogger log){

    string jsonContent1;
   PropertiesEncrypt PropEnc = new PropertiesEncrypt();
   endpoint = endpoint + resource_id + "/transparentDataEncryption/current?api-version=2014-04-01";
   log.LogInformation("URL for API: "+endpoint);
   PropEnc.status =  "Enabled";
   EncryptionProp ep = new EncryptionProp();
   ep.properties = PropEnc;
   string jsonContent1 = JsonConvert.SerializeObject(ep);  

   return jsonContent1;
}

private string Enable_ThreatD(string endpoint,string resource_id, ILogger log){

   Properties prop = new Properties();
   endpoint = endpoint + resource_id + "/securityAlertPolicies/default?api-version=2014-04-01";
   log.LogInformation("endpoint: " + endpoint);            
   prop.state = "Enabled";
   prop.emailAccountAdmins = "Enabled";
   prop.emailAddresses = "abc@symantec.com";
   propobj p = new propobj();
   p.properties = prop;
   string jsonContent1 = JsonConvert.SerializeObject(p);
   return jsonContent1;
}


private string Threat_Retn90(string endpoint,string resource_id, ILogger log){

   Properties prop = new Properties();
   endpoint = endpoint + resource_id + "/securityAlertPolicies/default?api-version=2014-04-01";
   log.LogInformation("endpoint: " + endpoint);            
   prop.state = "Enabled";
   prop.emailAccountAdmins = "Enabled";
   prop.emailAddresses = "abc@symantec.com";
   prop.retentionDays = 95;
   propobj p = new propobj();
   p.properties = prop;
   string jsonContent1 = JsonConvert.SerializeObject(p);
   return jsonContent1;
}

private string Send_Alert(string endpoint, string resource_id, ILogger log){

   Properties prop = new Properties();
   endpoint = endpoint + resource_id + "/securityAlertPolicies/default?api-version=2014-04-01";
   log.LogInformation("endpoint: " + endpoint);            
   prop.state = "Enabled";
   prop.emailAccountAdmins = "Enabled";
   prop.emailAddresses = "abc@symantec.com";
   propobj p = new propobj();
   p.properties = prop;
   string jsonContent1 = JsonConvert.SerializeObject(p);
   return jsonContent1;
}

private string Detection_Type_All(string endpoint,string resource_id, ILogger log){
 
 Properties prop = new Properties();
   endpoint = endpoint + resource_id + "/securityAlertPolicies/default?api-version=2014-04-01";
   log.LogInformation("endpoint: " + endpoint);            
   prop.state = "Enabled";
   prop.emailAccountAdmins = "Enabled";
   prop.emailAddresses = "abc@symantec.com";
   prop.disabledAlerts = "";
   propobj p = new propobj();
   p.properties = prop;
   string jsonContent1 = JsonConvert.SerializeObject(p);
   return jsonContent1;
   
}

private string Email_Service(string endpoint,string resource_id, ILogger log){
   Properties prop = new Properties();
   endpoint = endpoint + resource_id + "/securityAlertPolicies/default?api-version=2014-04-01";
   log.LogInformation("endpoint: " + endpoint);            
   prop.state = "Enabled";
   prop.emailAccountAdmins = "Enabled";
   prop.emailAddresses = "abc@symantec.com";
   propobj p = new propobj();
   p.properties = prop;
   string jsonContent1 = JsonConvert.SerializeObject(p);
   return jsonContent1;
   
}


public static void Run(Tuple<string, string, string> tuple1, ILogger log)
{
   log.LogInformation("Activity function RemediateSQLDB started...");

   string module_id = tuple1.Item1;
   string resource_id = tuple1.Item2;
   string stoken = tuple1.Item3;
   // string resource_id = "/subscriptions/6bcc4190-5f36-4d8f-ae67-91edd41ad9d2/resourceGroups/CCS-Test-Resource-Group/providers/Microsoft.Sql/servers/testcwa/databases/TestPCSQLDB1";
   log.LogInformation("Remediation started for resource with ID: ");
   log.LogInformation(resource_id);

   var httpClient = new HttpClient();
   httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);

   string endpoint = "https://management.azure.com";
   string jsonContent="";

   if (module_id == "Data_Encryption"){
       jsonContent = Data_Encryption(endpoint, resource_id, log);
   }
   
   if (module_id == "Enable_ThreatD"){
       jsonContent = Enable_ThreatD(endpoint, resource_id, log);
   }

   if (module_id == "Threat_Retn90"){
       jsonContent = Threat_Retn90(endpoint, resource_id, log);
   }

   if (module_id == "Send_Alert"){
       jsonContent = Send_Alert(endpoint, resource_id, log);
   }

   if (module_id == "Detection_Type_All"){
       jsonContent = Detection_Type_All(endpoint, resource_id, log);
   }

   if (module_id == "Email_Service"){
       jsonContent = Email_Service(endpoint, resource_id, log);
   }
   

   if (jsonContent != ""){
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

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
}
