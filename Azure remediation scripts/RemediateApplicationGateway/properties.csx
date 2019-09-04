public class waf
{
      public bool enabled= true;
      public string firewallMode ;
      public string ruleSetType= "OWASP";
      public string ruleSetVersion= "3.0";
      public disRG[] disabledRuleGroups = new disRG[] {};
  //    string[] arr = new string[] {};
      public exc[] exclusions = new exc[] {};
      public bool requestBodyCheck= true;
      public int maxRequestBodySizeInKb= 128;
      public int fileUploadLimitInMb= 100;
}
public class disRG
{
      public string ruleGroupName;
      public int[] rules;
}
public class exc
{
      public string matchVariable;
      public string selector;
      public string selectorMatchOperator;
}
public class fw
{
      public string Detection = "Detection";
      public string Prevention;
}
