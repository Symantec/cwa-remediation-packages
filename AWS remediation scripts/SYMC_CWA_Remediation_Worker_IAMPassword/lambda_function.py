import json
import boto3
import sys
from raise_sns_notification import *

def lambda_handler(event, context):
    try:
        print('symc_cwa_remediation_worker_iampassword called')
        print(event)
        print('Remediating '+event['resource']['id']+' in region '+event['resource']['region'])
        
        generateAndSendRemediationOutput = False
        
        cloud_provider_config = {}
        for eventKeys in event:
            if(eventKeys == "cloud_provider_config"):
                generateAndSendRemediationOutput = True
                cloud_provider_config = event['cloud_provider_config']
                print (eventKeys)
                break

        if(generateAndSendRemediationOutput):
            print('Remediation Cloud Provider Configuration : ' + json.dumps(cloud_provider_config))
        else:
            print('Remediation Cloud Provider Configuration Not Provided')


        remediation_output_message = {}
        if(generateAndSendRemediationOutput):
            remediation_check_map = dict()
            remediation_output_message["payload_id"] = event['payload_id']
            remediation_output_message["remediated_checks"] = []
        
        change_length = False
        change_uppercase = False
        change_lowercase = False
        change_requirenumbers = False
        
        for check in event['checks']:
            
            if(check['id'].lower()=="c57a0315-c198-476c-be87-0f39ef128f55"):
                if(generateAndSendRemediationOutput):
                    remediation_check_obj = {}
                    remediation_check_map['c57a0315-c198-476c-be87-0f39ef128f55']=remediation_check_obj
                    remediation_check_obj['id']=check['id']
                    remediation_check_obj['status']=0
                    remediation_check_obj['version']=check['version']
                
                change_length = True
            
            if(check['id'].lower()=="e9a69a05-25ad-4893-a279-95ccc36395ed"):
                if(generateAndSendRemediationOutput):
                    remediation_check_obj = {}
                    remediation_check_map['e9a69a05-25ad-4893-a279-95ccc36395ed']=remediation_check_obj
                    remediation_check_obj['id']=check['id']
                    remediation_check_obj['status']=0
                    remediation_check_obj['version']=check['version']
                
                change_requirenumbers = True
            
            if(check['id'].lower()=="80513a32-b644-4f16-8151-f0c74482a2fb"):
                if(generateAndSendRemediationOutput):
                    remediation_check_obj = {}
                    remediation_check_map['80513a32-b644-4f16-8151-f0c74482a2fb']=remediation_check_obj
                    remediation_check_obj['id']=check['id']
                    remediation_check_obj['status']=0
                    remediation_check_obj['version']=check['version']
                
                change_uppercase = True
            
            if(check['id'].lower()=="5833062d-54f9-4154-9fea-27622117c9e7"):
                if(generateAndSendRemediationOutput):
                    remediation_check_obj = {}
                    remediation_check_map['5833062d-54f9-4154-9fea-27622117c9e7']=remediation_check_obj
                    remediation_check_obj['id']=check['id']
                    remediation_check_obj['status']=0
                    remediation_check_obj['version']=check['version']
                
                change_lowercase = True
                
        try:
            
            iam = boto3.resource('iam')
            print('Getting account password policy')
            account_password_policy = iam.AccountPasswordPolicy()
            print('Getting account password policy Successfull')
            
            print('Updating account password policy')
        
            response = account_password_policy.update(
                MinimumPasswordLength=15 if change_length else account_password_policy.minimum_password_length,
                RequireSymbols=account_password_policy.require_symbols,
                RequireNumbers=True if change_requirenumbers else account_password_policy.require_numbers,
                RequireUppercaseCharacters=True if change_uppercase else account_password_policy.require_uppercase_characters,
                RequireLowercaseCharacters=True if change_lowercase else account_password_policy.require_lowercase_characters,
                AllowUsersToChangePassword=account_password_policy.allow_users_to_change_password,
                MaxPasswordAge=account_password_policy.max_password_age,
                PasswordReusePrevention=account_password_policy.password_reuse_prevention,
                HardExpiry=account_password_policy.hard_expiry
            )
            
            if(generateAndSendRemediationOutput):
                for k in remediation_check_map:
                    if(k.lower() == "c57a0315-c198-476c-be87-0f39ef128f55"):
                        remediation_check_obj = remediation_check_map['c57a0315-c198-476c-be87-0f39ef128f55']
                        remediation_check_obj['status']=0
                        remediation_check_obj['message']='Remediated Succesfully from AWS Lambda'
                        remediation_output_message["remediated_checks"].append(remediation_check_obj)
                    if(k.lower() == "e9a69a05-25ad-4893-a279-95ccc36395ed"):
                        remediation_check_obj = remediation_check_map['e9a69a05-25ad-4893-a279-95ccc36395ed']
                        remediation_check_obj['status']=0
                        remediation_check_obj['message']='Remediated Succesfully from AWS Lambda'
                        remediation_output_message["remediated_checks"].append(remediation_check_obj)
                    if(k.lower() == "80513a32-b644-4f16-8151-f0c74482a2fb"):
                        remediation_check_obj = remediation_check_map['80513a32-b644-4f16-8151-f0c74482a2fb']
                        remediation_check_obj['status']=0
                        remediation_check_obj['message']='Remediated Succesfully from AWS Lambda'
                        remediation_output_message["remediated_checks"].append(remediation_check_obj)
                    if(k.lower() == "5833062d-54f9-4154-9fea-27622117c9e7"):
                        remediation_check_obj = remediation_check_map['5833062d-54f9-4154-9fea-27622117c9e7']
                        remediation_check_obj['status']=0
                        remediation_check_obj['message']='Remediated Succesfully from AWS Lambda'
                        remediation_output_message["remediated_checks"].append(remediation_check_obj)
                    
            print('Remediation successful.')
        
        except:

            print('Error occurred while calling password policy update API ' + str(sys.exc_info()[0]))
            errorMessage = str(sys.exc_info()[0])
            
            for k in remediation_check_map:
                if(k.lower() == "c57a0315-c198-476c-be87-0f39ef128f55"):
                    remediation_check_obj = remediation_check_map['c57a0315-c198-476c-be87-0f39ef128f55']
                    remediation_check_obj['status']=1
                    remediation_check_obj['message']=errorMessage
                    remediation_output_message["remediated_checks"].append(remediation_check_obj)
                if(k.lower() == "e9a69a05-25ad-4893-a279-95ccc36395ed"):
                    remediation_check_obj = remediation_check_map['e9a69a05-25ad-4893-a279-95ccc36395ed']
                    remediation_check_obj['status']=1
                    remediation_check_obj['message']=errorMessage
                    remediation_output_message["remediated_checks"].append(remediation_check_obj)
                if(k.lower() == "80513a32-b644-4f16-8151-f0c74482a2fb"):
                    remediation_check_obj = remediation_check_map['80513a32-b644-4f16-8151-f0c74482a2fb']
                    remediation_check_obj['status']=1
                    remediation_check_obj['message']=errorMessage
                    remediation_output_message["remediated_checks"].append(remediation_check_obj)
                if(k.lower() == "cb321f8d-f15b-4f52-8b0e-3c887167e41"):
                    remediation_check_obj = remediation_check_map['cb321f8d-f15b-4f52-8b0e-3c887167e41']
                    remediation_check_obj['status']=1
                    remediation_check_obj['message']=errorMessage
                    remediation_output_message["remediated_checks"].append(remediation_check_obj)
                    
        if(generateAndSendRemediationOutput):
            remediation_output_message_str = json.dumps(remediation_output_message)
            print('Remediation Output Message Generated : ' + remediation_output_message_str)
            remediation_output_message_str_json = prepareSNSJsonMessage(remediation_output_message_str)
            raise_sns_notification(cloud_provider_config['output_sns_topic_arn'],cloud_provider_config['output_sns_topic_region'],remediation_output_message_str_json,'json')
            print('Remediation Output Message Published Succesfully to SNS.')

        return {
            'statusCode': 200,
            'body': json.dumps('Remediation Performed Succesfully')
        }
    except:
        print ("Unexpected error:", str(sys.exc_info()[0]))    
        raise
