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



public static dynamic Email_Owner(string resource_id, ILogger log, string stoken, dynamic c)
{

    c.properties.securityContactConfiguration.securityContactPhone = "9898989898";
    c.properties.securityContactConfiguration.securityContactEmails = "a@b.com";
    c.properties.securityContactConfiguration.sendToAdminOn = true;   
    return c;
  
}

public static dynamic Phone_Set(string resource_id, ILogger log, string stoken, dynamic c)
{
    
    c.properties.securityContactConfiguration.securityContactPhone = "9898989898";
    return c;
    
}
public static dynamic Email_Set(string resource_id, ILogger log, string stoken, dynamic c)
{
  
    c.properties.securityContactConfiguration.securityContactEmails = "a@b.com";
    return c;
  
}
public static dynamic Vulnerability_Assessment(string resource_id, ILogger log, string stoken, dynamic c)
{
  
    c.properties.recommendations.vulnerabilityAssessment = "On";
   return c;
}
public static dynamic AAC_Enable(string resource_id, ILogger log, string stoken, dynamic c)
{

    c.properties.recommendations.appWhitelisting = "On";
    return c;
}
public static dynamic NGF_Enable(string resource_id, ILogger log, string stoken, dynamic c)
{
  
    c.properties.recommendations.ngfw = "On";
    return c;
}

public static dynamic JIT(string resource_id, ILogger log, string stoken, dynamic c)
{
   
    c.properties.recommendations.jitNetworkAccess = "On";
    return c;
   
}
public static dynamic Disk_Encryption(string resource_id, ILogger log, string stoken, dynamic c)
{
    
    c.properties.recommendations.diskEncryption = "On";
    return c;

}
public static dynamic WAF_Enable(string resource_id, ILogger log, string stoken, dynamic c)
{

    c.properties.recommendations.waf = "On";
    return c;
   
}

public static dynamic System_Update(string resource_id, ILogger log, string stoken, dynamic c)
{
    
    c.properties.recommendations.patch = "On";
    return c;

}

public static dynamic Security_Config(string resource_id, ILogger log, string stoken, dynamic c)
{

    c.properties.recommendations.baseline = "On";
    return c;

}

public static dynamic Endpoint_Protection(string resource_id, ILogger log, string stoken, dynamic c)
{
 
    c.properties.recommendations.antimalware = "On";
    return c;

}

public static dynamic Monitoring_Agent(string resource_id, ILogger log, string stoken, dynamic c)
{

    c.properties.logCollection = "On";
    return c;
 
}

public static dynamic Email_User(string resource_id, ILogger log, string stoken, dynamic c)
{

     c.properties.securityContactConfiguration.securityContactPhone = "9898989898";
    c.properties.securityContactConfiguration.securityContactEmails = "a@b.com";
    c.properties.securityContactConfiguration.areNotificationsOn = true;
return c;
}

public static void Run(Tuple<string, string, string[]> tuple1, ILogger log)
{
 
   log.LogInformation("Activity function started...");
  // string module_id = tuple1.Item1;
   string resource_id = tuple1.Item1;
   string stoken = tuple1.Item2;
   
   //List<string> check_list = tuple1.Item3;
    string[] check_list;
    check_list = tuple1.Item3;
   
   log.LogInformation("Token: " + stoken);
    
   log.LogInformation("Remediation started for resource with ID: ");
   log.LogInformation(resource_id);
   
    string endpoint = "https://management.azure.com/";
    endpoint = endpoint + resource_id + "?api-version=2015-06-01-preview";
    string httpResponseBody = GetPolicy(resource_id,log,stoken,endpoint);
    dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
 
    dynamic x = c;
   
   for(int i=0;i<check_list.Length;i++)
   {
       log.LogInformation(check_list[i]);
   }
    foreach(string check in check_list)
    {
        log.LogInformation(check);
        
        if(check=="Email_Owner")
            x = Email_Owner(resource_id,log,stoken,c);
            
        else if(check=="Phone_Set")
            x= Phone_Set(resource_id,log,stoken,c);

        else if(check=="Email_Set")
                x= Email_Set(resource_id,log,stoken,c);

        else if(check=="Vulnerability_Assessment")
                x= Vulnerability_Assessment(resource_id,log,stoken,c);

        else if(check=="AAC_Enable")
                x= AAC_Enable(resource_id,log,stoken,c);   

        else if(check=="NGF_Enable")
                x= NGF_Enable(resource_id,log,stoken,c);  
        
        else if(check=="JIT")
                x= JIT(resource_id,log,stoken,c);   

        else if(check=="Disk_Encryption")
                x= Disk_Encryption(resource_id,log,stoken,c);  

        else if(check=="WAF_Enable")
            x= WAF_Enable(resource_id,log,stoken,c);     

        else if(check=="Email_User")
                x= Email_User(resource_id,log,stoken,c);   
                
        else if(check=="Security_Config")
                x= Security_Config(resource_id,log,stoken,c); 
        
        else if(check=="System_Update")
                x= System_Update(resource_id,log,stoken,c); 

        else if(check=="Endpoint_Protection")
                x= Endpoint_Protection(resource_id,log,stoken,c); 

        else if(check=="Monitoring_Agent")
                x= Monitoring_Agent(resource_id,log,stoken,c); 

    c = x;
    }
           SetPolicy(c,stoken,log,endpoint);
 
}
