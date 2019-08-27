//test compilation1
#r "Microsoft.Azure.WebJobs.Extensions.DurableTask"
#r "Newtonsoft.Json"
#load "model.csx"

using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class ClientDetail
{
    public string grant_type = "client_credentials";
    public string client_id =  Environment.GetEnvironmentVariable("CLIENT_ID", EnvironmentVariableTarget.Process);
    public string client_secret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
    public string resource = "https://management.azure.com/";
     public string tenant_id = Environment.GetEnvironmentVariable("DIRECTORY_ID");

}

class Result {
    public string token_type {get;set;}
    public int expires_in {get;set;}
    public int ext_expires_in {get;set;}
    public string expires_on{get;set;}
    public string not_before {get;set;}
    public string resource {get;set;}
    public string access_token {get;set;}
}

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

private static Tuple<string, string> ReadCheckConfig(string check_id, string path, ILogger log)
{
  log.LogInformation("Parsing check config file: "); 
  var result1 = Tuple.Create("", "");

  FileStream fsSource = new FileStream(path, FileMode.Open, FileAccess.Read);
  using (StreamReader sr = new StreamReader(fsSource))
  {
      while(!sr.EndOfStream)
      {
       string line = sr.ReadLine();
       log.LogInformation(line);
       bool ignoreCaseSearchResult = line.StartsWith(check_id);
       log.LogInformation("Search for check_id "+check_id+" is :"+ignoreCaseSearchResult);
       if (ignoreCaseSearchResult)
        {
           log.LogInformation("Check "+ check_id +" found in config file");
           var fields = line.Split(':');
           string f0 = fields[0].Trim();
           log.LogInformation("Check ID: "+ f0);
           string f1 = fields[1].Trim();
           log.LogInformation("Function App: "+ f1);
           string f2 = fields[2].Trim();
           log.LogInformation("Function Module: "+ f2);

           result1 = Tuple.Create(f1, f2);
           return result1;
        }
      }

      log.LogInformation("Error: The Check Config file is Empty!!");
      return result1;
    }  
}

public static bool HasValue( Tuple<string, string> tuple)
    {
        return !string.IsNullOrEmpty(tuple?.Item1) && !string.IsNullOrEmpty(tuple?.Item2);
    }

private static string GetResourceType(string resourceid)
{ 
    string resourcetype = "could not found";
    if(resourceid.Contains("Microsoft.Sql")){
        if (resourceid.Contains("/servers/"))
        {
            resourcetype = "SQL_Server";
            return resourcetype;
        }
        if (resourceid.Contains("/database/"))
        {
            resourcetype = "SQL_DB";
            return resourcetype;
        }

    }
    return resourcetype;
}

private static List<string> GetCheckIDs(List<Check> checks){
    var checkList = new List<string>();
    foreach (Check check in checks)
    {
      string checkid = check.id;
      checkList.Add(checkid);
    }
    return checkList;
}

public static void Run(DurableOrchestrationContext context, ILogger log, ExecutionContext context1)
{
  var event_data = context.GetInput<string>();
  var outputs = new List<string>();
   

  //Check for event_data
  PayloadStruct data = JsonConvert.DeserializeObject<PayloadStruct>(event_data);
  log.LogInformation("Payload ID : " + data.payload.payload_id);
  log.LogInformation("Resource account : " + data.payload.resource.account_id);
  log.LogInformation("Resource Name : " + data.payload.resource.name);
  log.LogInformation("Resource region : " + data.payload.resource.region);
  string resource_type = GetResourceType(data.payload.resource.id);
  log.LogInformation("Resource Type : " + resource_type);

  string resource_id = data.payload.resource.id; 
  List<Check> checks = data.payload.checks;
  List<string> checklist = GetCheckIDs(checks);

  //get the path of check config file
  var path = System.IO.Path.Combine(context1.FunctionDirectory, "check_config.csv");  
  log.LogInformation("Check Config file path: "+ path); 

  log.LogInformation("Generating access token: ");
  string stoken = GetAccessToken(log);
  log.LogInformation("Token: "+ stoken);
  
  foreach (string check in checklist){
     log.LogInformation("Check Traced: "+ check);
     Tuple<string,string> getFunctionApp = ReadCheckConfig(check, path, log);
     string functionapp = getFunctionApp.Item1;
     string moduleapp = getFunctionApp.Item2;
     bool b1 = HasValue(getFunctionApp);

     if(b1){

         log.LogInformation("Calling Function : "+ functionapp);
         var tuple1 = Tuple.Create(moduleapp, resource_id, stoken);
         context.CallActivityAsync(functionapp, tuple1);
     }else{
         log.LogInformation("Error: Pre-requisite Function App or Module Name missing!!");
     }
     /*test*/
    }

     


}
