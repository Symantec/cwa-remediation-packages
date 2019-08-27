public class EncryptionProp
{
    public PropertiesEncrypt properties;
}

public class PropertiesEncrypt
{
    public string status;
}

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

class Result {
    public string token_type {get;set;}
    public int expires_in {get;set;}
    public int ext_expires_in {get;set;}
    public string expires_on{get;set;}
    public string not_before {get;set;}
    public string resource {get;set;}
    public string access_token {get;set;}
}
