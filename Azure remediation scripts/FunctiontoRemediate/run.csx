#r "Microsoft.Azure.EventGrid"
#r "Newtonsoft.Json"

using Microsoft.Azure.EventGrid.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Configuration;
using System.Security.Claims; 
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class propobj
{
    public Properties properties;
}

public class Properties
{    
    public string state {get; set;}
    public string emailAccountAdmins {get; set;}
    public string emailAddresses {get; set;}
    public string disabledAlerts {get; set;}
    public int    retentionDays {get; set;}
    public string storageAccountAccessKey {get; set;}
    public string storageEndpoint {get; set;}
    public string useServerDefault {get; set;}
}

private class Data
{
    public Payload payload;
}

private class Payload
{
    public string payload_id {get;set;}
    public List<Check> checks;
    public Resource    resource;
}

private class Check
{
    public string     severity {get;set;}
    public string     name{get;set;}
    public string     id {get;set;}
    public string     scan_time {get;set;}
    public string     version {get;set;}
}

private class Resource
{
    public    string   account_id  {get;set;}
    public    string   name        {get;set;}
    public    string   id          {get;set;}
    public    string   region      {get;set;}
}

class EncryptionProp
{
    public PropertiesEncrypt properties;
}

class PropertiesEncrypt
{
    public string status;
}

class ClientDetail
{
    public string grant_type = "client_credentials";
    public string client_id  = "54ae88e1-35f3-4342-9f53-514e21b8992e";
    public string client_secret = "uwp3LbOjWHmoT9bZKlSJm77VCZKDnm0Fdc+zyCXmyRk=";
    public string resource = "https://management.azure.com/";
    public string tenant_id = "3b217a9b-6c58-428b-b022-5ad741ce2016";
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

private static string GetAccessToken()
{
    ClientDetail cd = new ClientDetail();    

    var keyValues = new List<KeyValuePair<string, string>>();
    keyValues.Add(new KeyValuePair<string, string>("grant_type", cd.grant_type));
    keyValues.Add(new KeyValuePair<string, string>("client_id", cd.client_id));
    keyValues.Add(new KeyValuePair<string, string>("client_secret", cd.client_secret));
    keyValues.Add(new KeyValuePair<string, string>("resource", cd.resource));

    var httpClient = new HttpClient();
    var response = httpClient.PostAsync("https://login.microsoftonline.com/" + cd.tenant_id +"/oauth2/token", new FormUrlEncodedContent(keyValues)).Result;
    string result = response.Content.ReadAsStringAsync().Result.ToString();    
    Result rs = JsonConvert.DeserializeObject<Result>(result);
    return rs.access_token;    
}

public static void Run(EventGridEvent eventGridEvent, ILogger log)
{
    log.LogInformation(eventGridEvent.Data.ToString());
    
    Data data = JsonConvert.DeserializeObject<Data>(eventGridEvent.Data.ToString());
    log.LogInformation("Payload ID" + data.payload.payload_id);
    log.LogInformation("Check ID" + data.payload.checks[0].id);
    log.LogInformation("Check name" + data.payload.checks[0].name);
    log.LogInformation("Resource account" + data.payload.resource.account_id);
    log.LogInformation("Resource Name" + data.payload.resource.name);
    log.LogInformation("Resource ID" + data.payload.resource.id);
    log.LogInformation("Resource region" + data.payload.resource.region);
    
    string stoken = GetAccessToken();             

    //log.LogInformation("Token:" + stoken);

    var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + stoken);
    
    
    foreach (Check check in data.payload.checks)
    {
        string endpoint = "https://management.azure.com";
        string jsonContent;
        if(string.Equals(check.id,"A57A6985-D966-466F-BA2E-7DE1FE62B58B",StringComparison.CurrentCultureIgnoreCase))
        {
            PropertiesEncrypt PropEnc = new PropertiesEncrypt();
            endpoint = endpoint + data.payload.resource.id + "/transparentDataEncryption/current?api-version=2014-04-01";
            PropEnc.status =  "Enabled";
            EncryptionProp ep = new EncryptionProp();
            ep.properties = PropEnc;
            jsonContent = JsonConvert.SerializeObject(ep);            
        }
        else
        {
            Properties prop = new Properties();
            endpoint = endpoint + data.payload.resource.id + "/securityAlertPolicies/default?api-version=2014-04-01";
            log.LogInformation("endpoint: " + endpoint);            
            prop.state = "Enabled";
            prop.emailAccountAdmins = "Enabled";
            prop.emailAddresses = "abc@symantec.com";
            if(string.Equals(check.id,"45797DFA-FE81-4227-9084-FD792326E1C2",StringComparison.CurrentCultureIgnoreCase))
            {                                    
                prop.state = "Enabled";
            }
            if(string.Equals(check.id,"EF31C502-2BAE-4B6A-B3B3-60D39183B351",StringComparison.CurrentCultureIgnoreCase))        
            {
                prop.retentionDays = 95;
                //prop.storageAccountAccessKey = "Storageaccountaccesskey"; Valid key
                //prop.storageEndpoint = "StorageEndPoint"; Valid Account
            }
            if(string.Equals(check.id,"1CD910DD-9BFA-41BD-AE4A-64662536FE77",StringComparison.CurrentCultureIgnoreCase))
            {
                prop.emailAccountAdmins = "Enabled";
                prop.emailAddresses = "abc@symantec.com";
            }
            if(string.Equals(check.id,"778A0216-90AA-47A5-9FD4-8FFF82823D9F",StringComparison.CurrentCultureIgnoreCase))
            {
                prop.disabledAlerts = "";            
            }
            if(string.Equals(check.id,"B1D6717E-11CE-43BE-97EF-2A01892E507F",StringComparison.CurrentCultureIgnoreCase))
            {
                prop.emailAccountAdmins = "Enabled";
                prop.emailAddresses = "abc@symantec.com";
            }

            propobj p = new propobj();
            p.properties = prop;
            jsonContent = JsonConvert.SerializeObject(p);            
        }

        
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = httpClient.PutAsync(endpoint, content).Result;
        string result = response.Content.ReadAsStringAsync().Result.ToString();

        log.LogInformation("SQL setting:" + result);        
    }        
}
