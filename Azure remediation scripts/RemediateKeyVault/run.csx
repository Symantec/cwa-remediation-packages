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


private static string GetAccessToken(ILogger log1)
{
    ClientDetail cd = new ClientDetail();    
    
    log1.LogInformation("Enter Get Access Token Function!!");
    var keyValues = new List<KeyValuePair<string, string>>();
    keyValues.Add(new KeyValuePair<string, string>("grant_type", cd.grant_type));

    log1.LogInformation("client_id :" +cd.client_id);
    keyValues.Add(new KeyValuePair<string, string>("client_id", cd.client_id));

    log1.LogInformation("client_secret :" +cd.client_secret);
    keyValues.Add(new KeyValuePair<string, string>("client_secret", cd.client_secret));
    keyValues.Add(new KeyValuePair<string, string>("resource", cd.resource));

    var httpClient = new HttpClient();
    log1.LogInformation("https://login.microsoftonline.com/" + cd.tenant_id +"/oauth2/token");
    var response = httpClient.PostAsync("https://login.microsoftonline.com/" + cd.tenant_id +"/oauth2/token", new FormUrlEncodedContent(keyValues)).Result;
    log1.LogInformation("response :" +response);
    string result = response.Content.ReadAsStringAsync().Result.ToString(); 
    log1.LogInformation("result :" +result); 
    Result rs = JsonConvert.DeserializeObject<Result>(result);
    return rs.access_token;    
}

private static string get_key_secret(string resource_id,string stoken, ILogger log, string resource_name,string endpoint)
{
        var httpCl = new HttpClient();
    httpCl.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);
    log.LogInformation("URL for API: "+endpoint);
      var httpResponse = new HttpResponseMessage();
      string httpResponseBody = "";
      httpResponse = httpCl.GetAsync(endpoint).Result;
      httpResponse.EnsureSuccessStatusCode();
      httpResponseBody = httpResponse.Content.ReadAsStringAsync().Result.ToString();
      log.LogInformation(httpResponseBody);
    return httpResponseBody;
  
}

private static void update_exp(string resource_id, string stoken, List<string> idList, ILogger log)
{
  var httpCl = new HttpClient();
  httpCl.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);

  var httpResponse = new HttpResponseMessage();
  string httpResponseBody = "";

  foreach (string o in idList)
    {
       log.LogInformation(o);
       var ep = o + "?api-version=7.0";

       httpResponse = httpCl.GetAsync(ep).Result;
       httpResponse.EnsureSuccessStatusCode();
       httpResponseBody = httpResponse.Content.ReadAsStringAsync().Result.ToString();
  
       dynamic c = JsonConvert.DeserializeObject(httpResponseBody);
       if(c.attributes.exp==null)
       {
            c.attributes.exp = 31556952;
       }
       var content = new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json");
       var response = httpCl.PatchAsync(ep, content).Result;

    }
}
public static void Key_Exp(string resource_id, ILogger log,string stoken,string resource_name){


  string url = "https://" + resource_name + ".vault.azure.net/keys/?api-version=7.0";
  string body = get_key_secret(resource_id,stoken,log,resource_name,url);
  var kidList = new List<string>();
    var obj = JsonConvert.DeserializeObject<keyv>(body);
    foreach (keys k in obj.value)
    {
      string kk = k.kid;
      kidList.Add(kk);
    }
  update_exp(resource_id,stoken,kidList,log);
   
}

public static void Secret_Exp(string resource_id, ILogger log,string stoken,string resource_name){

  string url = "https://" + resource_name + ".vault.azure.net/secrets/?api-version=7.0";
  string body = get_key_secret(resource_id,stoken,log,resource_name,url);

  var obj = JsonConvert.DeserializeObject<secretv>(body);
    var sidList = new List<string>();
    foreach (secrets s in obj.value)
    {
      string ss = s.id;
      sidList.Add(ss);
    }
  update_exp(resource_id,stoken,sidList,log);
   
}

private static List<string> GetKeyIDs(List<keys> val){
    var kidList = new List<string>();
    foreach (keys k in val)
    {
      string kk = k.kid;
      kidList.Add(kk);
    }
    return kidList;
}

private static List<string> GetSecretIDs(List<secrets> val){
    var sidList = new List<string>();
    foreach (secrets s in val)
    {
      string ss = s.id;
      sidList.Add(ss);
    }
    return sidList;
}



public static void Run(Tuple<string, string, string> tuple1, ILogger log)
{
   log.LogInformation("Activity function started...");
   string module_id = tuple1.Item1;
   string resource_id = tuple1.Item2;
   string resource_name = tuple1.Item3;
   
   string stoken = GetAccessToken(log);

   log.LogInformation("Remediation started for resource with ID: ");
   log.LogInformation(resource_id);
   log.LogInformation("Token for vault: " + stoken);
   log.LogInformation("Module ID: " + module_id);

   log.LogInformation("Resource Name: " + resource_name);
  if(module_id == "Key_Exp")
  {
   Key_Exp(resource_id,log,stoken,resource_name);
  }
  else
  {
    Secret_Exp(resource_id,log,stoken,resource_name);
  }

}
