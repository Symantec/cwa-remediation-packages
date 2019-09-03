public class keyv
{
    public List<keys> value;
    public string nextLink;
}

public class keys
{
    public string kid;
    public att attributes;
}

public class att
{
    public bool enabled;
    public int exp;
    public int created;
    public int updated;
    public string recoveryLevel;
}

class ClientDetail
{
    public string grant_type = "client_credentials";
    public string client_id =  Environment.GetEnvironmentVariable("CLIENT_ID", EnvironmentVariableTarget.Process);
    public string client_secret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
    public string resource = "https://vault.azure.net";
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
