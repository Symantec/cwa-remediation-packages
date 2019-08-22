
    public class Payload
    {
        public string payload_id {get;set;}
        public List<Check> checks;
        public Resource    resource;
    }

    public class Check
    {
        public string     severity {get;set;}
        public string     name{get;set;}
        public string     id {get;set;}
        public string     scan_time {get;set;}
        public string     version {get;set;}
    }



    public class Resource
   {
      public    string   account_id  {get;set;}
      public    string   name        {get;set;}
      public    string   id          {get;set;}
      public    string   region      {get;set;}
   }
